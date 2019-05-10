using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FreeSql;
using FreeSqlDemo.Bussiness.DTO.Login;
using FreeSqlDemo.Bussiness.DTO.Role;
using FreeSqlDemo.Bussiness.DTO.Terant;
using FreeSqlDemo.Bussiness.DTO.User;
using FreeSqlDemo.Bussiness.Entities;
using FreeSqlDemo.Domain.Entities;
using FreeSqlDemo.Domain.Repository;
using FreeSqlDemo.Infrastructure.DomainBase;
using FreeSqlDemo.Infrastructure.Entity;
using FreeSqlDemo.Infrastructure.Entity.Page;
using FreeSqlDemo.Infrastructure.JWTOptions;
using FreeSqlDemo.Infrastructure.RepositoryBase;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FreeSqlDemo.Bussiness.Service
{

    public class CommonService : DomainBase, ICommonService
    {
        private readonly IUserRepository _userRep;
        private readonly IRoleRepository _roleRep;
        private readonly ICache _cache;
        private readonly JwtOptions _jwtOptions;
        private readonly BaseRepository<Terant> _terantRep;
        public CommonService(IServiceProvider service, ILogger<CommonService> logger) : base(service, logger)
        {
            _userRep = service.GetRequiredService<IUserRepository>();
            _roleRep = service.GetRequiredService<IRoleRepository>();
            _cache = service.GetRequiredService<IFreeSql>().Cache;
            _jwtOptions = service.GetRequiredService<IOptions<JwtOptions>>().Value;
            _terantRep = UnitOfWork.GetRepository<Terant>();
            var codeFirst = service.GetService<IFreeSql>().CodeFirst;

            #region 使用Sqllit可能会锁表所以索性手动迁移一下防止锁表 其他数据库应该不会出现这个问题
            codeFirst.SyncStructure<User>();
            codeFirst.SyncStructure<Role>();
            codeFirst.SyncStructure<UserRole>();
            codeFirst.SyncStructure<Terant>();
            codeFirst.SyncStructure<UserInfo>();
            #endregion

        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<LoginResult> Login(LoginDto dto)
        {
            var res = new LoginResult();
            if (_userRep.Select.Where(x => x.Account == dto.Account).Count() > 0)
            {
                var userid = await Validate(dto.Account, dto.PassWord);
                res.Subject = userid.ToString();
                if (userid == -1)
                {
                    res.Code = LoginCode.PasswordError;
                    return res;
                }
                var user = await GetUserByIdAsync(userid);
                res.AccessToken = GetAccessToken(userid);
                //我这里演示一下缓存的特殊用法 
                await _cache.SetAsync<CurrentUser>($"{CacheKey.Login}:{user.Id}",
                    Mapper.Map<CurrentUser>(user), 30 * 60);
                res.Code = LoginCode.Success;
            }
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LogOut()
        {
            await _cache.RemoveAsync($"{CacheKey.Login}:{CurrentUser.Id}");
            return true;

        }
        /// <summary>
        /// 获取用户 通过Id
        /// ##FreeSql LeftJoin演示
        /// ##多对多分步查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User> GetUserByIdAsync(int id)
        {

            var user = await _userRep.Select
                .Where(x => x.TerantId > 0 && x.UserInfo_Id > 0)//这种是延迟加载的触发
                ////下面是Leftjoin的两种用法
                //.LeftJoin<UserRole>((ue, ur) => ue.Id == ur.UserId)//这里直接使用泛型
                //.LeftJoin(ue => ue.Terant.Id == ue.TerantId)//这里利用导航属性
                //.LeftJoin(x => x.UserInfo_Id == x.UserInfo.Id)
                .Where(x => x.Id == id)
                .ToOneAsync();
            var roleids = user.UserRoles.Select(ur => ur.RoleId);
            //这里第三层不能直接查出来 直接分步查询
            var roles = await _roleRep.Select.Where(r => roleids.Contains(r.Id)).ToListAsync();
            foreach (var ur in user.UserRoles)
            {
                ur.Role = roles.Find(x => x.Id == ur.RoleId);
            }
            return user;
        }

        public async Task<bool> AddUser(UserInput input)
        {
            var res = true;
            //多租户当然要验证一下租户是否存在咯
            _ = await _terantRep.Select.Where(x => x.Id == input.TerantId).ToOneAsync()
                ?? throw new Exception("租户Id不存在");
            //Automapper转一手
            var user = Mapper.Map<User>(input);
            //我这里已经在Automapper将数据转换过来了所以直接添加了
            await UnitOfWork.GetRepository<UserInfo>().InsertAsync(user.UserInfo);
            //这里把关联Id写上 因为FreeSql添加到Sql中时会自动将主键补齐所以直接用就行了
            user.UserInfo_Id = user.UserInfo.Id;
            user.UserRoles = new List<UserRole>();
            var userSave = await _userRep.InsertAsync(user);
            if (input.UserRoles?.Count > 0)
            {
                //我这里会先验证角色数据的正确性在添加 如果你不想 也可以不验证
                var roles = await _roleRep.Select.Where(r => input.UserRoles.Contains(r.Id)).ToListAsync();
                var insertRoles = new List<UserRole>();
                foreach (var role in roles)
                {
                    insertRoles.Add(new UserRole() { UserId = userSave.Id, RoleId = role.Id });
                }
                //持久化多对多
                await UnitOfWork.GetRepository<UserRole>().InsertAsync(insertRoles);
            }
            //打上租户Id
            user.UserInfo.TerantId = user.TerantId;

            try
            {
                //这里利用Uow做事务性提交 一次性提交所有数据
                UnitOfWork.Commit();
            }
            catch (Exception e)
            {
                res = false;
                Logger.LogError(e.ToString());
                UnitOfWork.Rollback();
            }
            return res;

        }

        /// <summary>
        /// 用户获取
        /// 这里主演演示内容：
        /// FreeSql：
        /// 分页
        /// WhereIf
        /// 一对一 多对多查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<PageListBase<UserVO>> GetUserPageAsync(UserPageParam param)
        {
            long total = 0;
            var userList = await _userRep.Select
                .WhereIf(!string.IsNullOrWhiteSpace(param.KeyWord),
                    x => param.KeyWord.Contains(x.RealName))
                .WhereIf(param.RoleIds?.Any() ?? false, x => x.UserRoles.Any(r => param.RoleIds.Contains(r.RoleId)))
                .Where(x => x.Terant.Id > 0 && x.UserInfo.Id > 0)//触发两个一对一表的延时加载
                .Count(out total)
                .OrderBy(param.OrderBy)
                .Page(param.PageIndex + 1, param.PageSize)
                .ToListAsync(true);
            if (userList.Any())
            {
                var userIds = userList.Select(a => a.Id).ToArray();
                var userRoleRep = _userRep.Orm.GetRepository<UserRole, int>();
                var userRoles = await userRoleRep.Select
                    .Where(ur => userIds.Contains(ur.UserId) && ur.Role.Id > 0)
                    .ToListAsync(true);
                foreach (var user in userList)
                {
                    user.UserRoles = userRoles.Where(a => a.UserId == user.Id).ToList();
                }
            }
            return new PageListBase<UserVO>(Mapper.Map<List<UserVO>>(userList), total, param.PageIndex + 1, param.PageSize); ;
        }



        /// <summary>
        /// 添加租户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> AddTerant(TerantInput input)
        {
            var entity = Mapper.Map<Terant>(input);
            entity.Effective = 3;
            await UnitOfWork.GetRepository<Terant>().InsertAsync(entity);
            try
            {
                UnitOfWork.Commit();
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                UnitOfWork.Rollback();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 角色分页查询
        /// </summary>
        /// <returns></returns>
        public async Task<PageListBase<RoleVO>> GetRolePageAsync(RolePageParam param)
        {
            long total = 0;
            var list = await _roleRep.Select
                  .WhereIf(!string.IsNullOrWhiteSpace(param.KeyWord), r => r.FullName.Contains(param.KeyWord))
                  .Count(out total)
                  .OrderBy(param.OrderBy)
                  .Page(param.PageIndex + 1, param.PageSize)
                  .ToListAsync(true);
            //分两步查询出对应的用户
            if (list.Any())
            {
                var rids = list.Select(x => x.Id);
                var userRoleRep = _userRep.Orm.GetRepository<UserRole, int>();//通过中间表加载出用户
                var userRoles = await userRoleRep.Select
                    .Where(ur => ur.User.Id > 0)//触发用户表关联
                    .Where<Role>(r => rids.Contains(r.Id))
                    .ToListAsync(true);
                foreach (var role in list)
                {
                    role.UserRoles = userRoles.Where(ur => ur.RoleId == role.Id).ToList();
                }
            }
            return new PageListBase<RoleVO>(Mapper.Map<List<RoleVO>>(list), total, param.PageIndex + 1, param.PageSize);

        }
        /// <summary>
        /// 角色新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> AddRole(RoleInput input)
        {
            var role = Mapper.Map<Role>(input);
            role.TerantId = CurrentUser.Terant.Id;
            await _roleRep.InsertAsync(role);
            var users = await _userRep.Select
                .WhereIf(
                    input.UserIds?.Any() ?? false,
                    u => !u.IsDeleted && input.UserIds.Contains(u.Id))
                  .ToListAsync();
            role.UserRoles = new List<UserRole>();
            foreach (var user in users)
            {
                role.UserRoles.Add(new UserRole()
                {
                    RoleId = role.Id,
                    UserId = user.Id
                });
            }
            await UnitOfWork.GetRepository<UserRole>().InsertAsync(role.UserRoles);
            try
            {
                UnitOfWork.Commit(); return true;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                Logger.LogError(e.ToString());
                return false;
            }
        }



        #region PrivateFunc
        /// <summary>
        /// 这里我就不加密了
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<int> Validate(string account, string password)
        {
            var userId = (await _userRep.Select.Where(x => x.Account == account && x.Password == password).ToOneAsync())?.Id;
            return userId ?? -1;
        }

        /// <summary>
        /// 生成jwt
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GetAccessToken(int userId)
        {
            var claims = new[]
            {
                new Claim(ClaimsType.Subject, userId.ToString()),
                new Claim(ClaimsType.UserId,userId.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.IssuerSigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.ValidIssuer,
                audience: _jwtOptions.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        #endregion

    }
}
