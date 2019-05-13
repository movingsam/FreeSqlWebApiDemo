using System;
using FreeSqlDemo.Bussiness.Entities;
using FreeSqlDemo.Infrastructure.Repository;

namespace FreeSqlDemo.Bussiness.Repository
{
    public class UserRepository : UnitOfWorkRepository<User, int>,IUserRepository
    {
        public UserRepository(IServiceProvider service) : base(service)
        {
        }
    }
}
