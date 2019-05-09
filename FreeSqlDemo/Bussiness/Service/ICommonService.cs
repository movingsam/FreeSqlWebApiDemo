using System.Threading.Tasks;
using FreeSqlDemo.Bussiness.DTO.Login;
using FreeSqlDemo.Bussiness.DTO.Terant;
using FreeSqlDemo.Bussiness.DTO.User;
using FreeSqlDemo.Domain.Entities;
using FreeSqlDemo.Infrastructure.DomainBase;

namespace FreeSqlDemo.Bussiness.Service
{
    public interface ICommonService:IDomainBase
    {
        Task<LoginResult> Login(LoginDto dto);
        Task<User> GetUserByIdAsync(int id);
        Task<bool> AddUser(UserInput input);
        Task<bool> AddTerant(TerantInput input);

    }
}