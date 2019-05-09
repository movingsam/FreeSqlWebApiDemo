using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSqlDemo.Domain.Entities;
using FreeSqlDemo.Infrastructure.Repository;

namespace FreeSqlDemo.Domain.Repository
{
    public class RoleRepository : UnitOfWorkRepository<Role, int>, IRoleRepository
    {
        public RoleRepository(IServiceProvider service) : base(service)
        {
        }
    }
}
