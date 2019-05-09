using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSqlDemo.Domain.Entities;
using FreeSqlDemo.Infrastructure.Repository;

namespace FreeSqlDemo.Domain.Repository
{
    public class UserRepository : UnitOfWorkRepository<User, int>,IUserRepository
    {
        public UserRepository(IServiceProvider service) : base(service)
        {
        }
    }
}
