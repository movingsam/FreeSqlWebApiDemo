using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FreeSql;
using FreeSqlDemo.Bussiness.DTO.Login;
using FreeSqlDemo.Bussiness.DTO.Role;
using FreeSqlDemo.Bussiness.DTO.Terant;
using FreeSqlDemo.Bussiness.DTO.User;
using FreeSqlDemo.Bussiness.Entities;
using FreeSqlDemo.Bussiness.Repository;
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

        #region 用户相关




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
                .Include(x => x.Terant)
                .Include(x => x.UserInfo)
                .IncludeMany(u => u.UserRoles, then => then.Include(ur => ur.Role))
                .Where(x => x.Id == id)
                .ToOneAsync();
            return user;
        }

        public async Task<bool> AddUser(UserInput input)
        {
            var res = true;
            //多租户当然要验证一下租户是否存在咯
            _ = await _terantRep.Select
                    .WhereIf(input.TerantId != 0, x => x.Id == input.TerantId)
                    .WhereIf(input.TerantId == 0, x => x.Id == CurrentUser.Terant.Id).ToOneAsync()
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
        /// 这里主要演示内容：
        /// FreeSql：
        /// 分页
        /// WhereIf
        /// 一对一 多对多查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<PageListBase<UserVO>> GetUserPageAsync(UserPageParam param)
        {
            #region 贪婪加载 Include/IncludeMany 条件筛选 WhereIf 排序（字符串） OrderBy("string") 分页 Page() 返回条数 Count()  
            var userList = await _userRep.Select
                .Include(x => x.UserInfo)
                .IncludeMany(x => x.UserRoles, next => next.Include(ur => ur.Role))
                .Include(x => x.Terant)
                .WhereIf(!string.IsNullOrWhiteSpace(param.KeyWord),
                    x => param.KeyWord.Contains(x.RealName))
                .WhereIf(param.RoleIds?.Any() ?? false, x => x.UserRoles.Any(r => param.RoleIds.Contains(r.RoleId)))
                .WhereIf(param.IsDelete != null, x => x.IsDeleted == param.IsDelete)
                .Count(out var total)//total是long型的
                .OrderBy(param.OrderBy)
                .Page(param.PageIndex + 1, param.PageSize)
                .ToListAsync(true);
            #endregion
            return new PageListBase<UserVO>(Mapper.Map<List<UserVO>>(userList), total, param.PageIndex + 1, param.PageSize);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">更新模型</param>
        /// <returns></returns>
        public async Task<bool> UpdateUser(int id, UserUpdateInput input)
        {
            var user = await _userRep.Select.Include(x => x.UserInfo)
                .IncludeMany(x => x.UserRoles, then => then.Include(ur => ur.Role))
                .Where(x => x.Id == id).ToOneAsync();
            var resUser = Mapper.Map(input, user);
            var info = resUser.UserInfo;
            if (info != null)
            {
                info.Address = input.Address;
                info.Email = input.Email;
                info.PhoneNumber = input.PhoneNumber;
                await UnitOfWork.GetRepository<UserInfo>().UpdateAsync(info);
            }

            await _userRep.UpdateAsync(resUser);
            var (needAdd, needRemove) = AddAndRemove(user.UserRoles.Select(ur => ur.RoleId), input.UserRoles);
            var userRoleRep = UnitOfWork.GetRepository<UserRole>();
            var userRole = needAdd.Select(userId => new UserRole() { RoleId = resUser.Id, UserId = userId }).ToList();
            if (userRole.Any())
            {
                await userRoleRep.InsertAsync(userRole);
            }
            userRole.Clear();
            userRole.AddRange(needRemove?.Select(userId => new UserRole() { RoleId = resUser.Id, UserId = userId }));
            if (userRole.Any())
            {
                await userRoleRep.DeleteAsync(userRole);
            }
            try
            {
                UnitOfWork.Commit();
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                UnitOfWork.Rollback();
                return false;
            }
        }

        /// <summary>
        /// 批量禁用用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> DisableUser(int[] ids)
        {
            var users = ids.Select(id => new User() { Id = id, IsDeleted = true });
            var res = await _userRep.UpdateDiy.SetSource(users).UpdateColumns(x => x.IsDeleted).ExecuteAffrowsAsync();
            return res > 0;
        }

        #endregion

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

        #region 角色相关
        /// <summary>
        /// 角色分页查询
        /// 贪婪加载
        /// Include/IncludeMany
        /// </summary>
        /// <returns></returns>
        public async Task<PageListBase<RoleVO>> GetRolePageAsync(RolePageParam param)
        {
            #region  贪婪加载 Include/IncludeMany
            var list = await _roleRep.Select
                  .IncludeMany(x => x.UserRoles, next => next.Include(ur => ur.User).Include(ur => ur.Role))
                  .WhereIf(!string.IsNullOrWhiteSpace(param.KeyWord), r => r.FullName.Contains(param.KeyWord))
                  .WhereIf(param.IsDelete != null, x => x.IsDeleted == param.IsDelete)
                  .Count(out var total)
                  .OrderBy(param.OrderBy)
                  .Page(param.PageIndex + 1, param.PageSize)
                  .ToListAsync(true);
            #endregion
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
        /// <summary>
        /// 角色禁用
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DisableRole(IEnumerable<int> ids)
        {
            var roles = ids.Select(id => new Role() { Id = id, IsDeleted = true });
            var res = await _roleRep.UpdateDiy.SetSource(roles).UpdateColumns(x => x.IsDeleted).ExecuteAffrowsAsync();
            return res > 0;
        }
        /// <summary>
        /// 角色更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> RoleUpdate(int id, RoleUpdateInput input)
        {
            var dbRole = await _roleRep.Select.IncludeMany(r => r.UserRoles)
                .Where(x => !x.IsDeleted && x.Id == id)
                .ToOneAsync();
            if (dbRole == null) throw new Exception("未找到角色,请检查是否为当前租户的角色或角色是否启用");
            {
                var res = Mapper.Map(input, dbRole);
                await _roleRep.UpdateAsync(res);
                var userIds = dbRole.UserRoles.Select(x => x.UserId);
                if (input.UserIds.Any())
                {
                    var (needAdd, needRemove) = AddAndRemove(input.UserIds, userIds);
                    var userRoleRep = UnitOfWork.GetRepository<UserRole>();
                    var userRole = needAdd.Select(userId => new UserRole() { RoleId = res.Id, UserId = userId }).ToList();
                    if (userRole.Any())
                    {
                        await userRoleRep.InsertAsync(userRole);
                    }
                    userRole.Clear();
                    userRole.AddRange(needRemove.Select(userId => new UserRole() { RoleId = res.Id, UserId = userId }));
                    if (userRole.Any())
                    {
                        await userRoleRep.DeleteAsync(userRole);
                    }
                }
                try
                {
                    UnitOfWork.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    UnitOfWork.Rollback();
                    return false;
                }
            }
        }


        #endregion


        #region PrivateFunc
        /// <summary>
        /// 这里是Demo我就不加密了
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
            var cress = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.ValidIssuer,
                audience: _jwtOptions.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: cress);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        /// <summary>
        /// 通用方法 返回新增和删除的中间表Key集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="result"></param>
        /// <param name="old"></param>
        /// <returns></returns>
        private (IEnumerable<TKey> needAdd, IEnumerable<TKey> needRemove) AddAndRemove<TKey>(IEnumerable<TKey> result, IEnumerable<TKey> old)
        {
            var enumerable = result as TKey[] ?? result.ToArray();
            if (old != null)
            {
                var second = old as TKey[] ?? old.ToArray();
                var noActionKey = enumerable.Intersect(second);
                var actionKey = noActionKey as TKey[] ?? noActionKey.ToArray();
                var needAdd = enumerable.Except(actionKey);
                var needRemove = second.Except(actionKey);
                return (needAdd, needRemove);
            }
            else
            {
                return (enumerable, new List<TKey>());
            }
        }

        #endregion

    }
}
