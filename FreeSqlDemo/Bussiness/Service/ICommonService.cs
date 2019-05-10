using System.Collections.Generic;
using System.Threading.Tasks;
using FreeSqlDemo.Bussiness.DTO.Login;
using FreeSqlDemo.Bussiness.DTO.Role;
using FreeSqlDemo.Bussiness.DTO.Terant;
using FreeSqlDemo.Bussiness.DTO.User;
using FreeSqlDemo.Domain.Entities;
using FreeSqlDemo.Infrastructure.DomainBase;
using FreeSqlDemo.Infrastructure.Entity.Page;

namespace FreeSqlDemo.Bussiness.Service
{
    public interface ICommonService : IDomainBase
    {
        Task<LoginResult> Login(LoginDto dto);
        Task<User> GetUserByIdAsync(int id);
        Task<bool> AddUser(UserInput input);
        Task<PageListBase<UserVO>> GetUserPageAsync(UserPageParam param);
        Task<bool> AddTerant(TerantInput input);
        Task<bool> LogOut();
        Task<bool> AddRole(RoleInput input);
        Task<PageListBase<RoleVO>> GetRolePageAsync(RolePageParam param);

    }
}