using System.Collections.Generic;
using System.Threading.Tasks;
using FreeSqlDemo.Bussiness.DTO.Login;
using FreeSqlDemo.Bussiness.DTO.Role;
using FreeSqlDemo.Bussiness.DTO.Terant;
using FreeSqlDemo.Bussiness.DTO.User;
using FreeSqlDemo.Bussiness.Entities;
using FreeSqlDemo.Infrastructure.DomainBase;
using FreeSqlDemo.Infrastructure.Entity.Page;

namespace FreeSqlDemo.Bussiness.Service
{
    public interface ICommonService : IDomainBase
    {
        Task<LoginResult> Login(LoginDto dto);
        Task<User> GetUserByIdAsync(int id);
        Task<bool> AddUser(UserInput input);
        Task<bool> UpdateUser(int id, UserUpdateInput input);
        Task<bool> DisableUser(int[] ids);
        Task<PageListBase<UserVO>> GetUserPageAsync(UserPageParam param);
        Task<bool> AddTerant(TerantInput input);
        Task<bool> LogOut();
        Task<bool> AddRole(RoleInput input);
        Task<PageListBase<RoleVO>> GetRolePageAsync(RolePageParam param);
        Task<bool> DisableRole(IEnumerable<int> ids);
        Task<bool> RoleUpdate(int id, RoleUpdateInput input);

    }
}