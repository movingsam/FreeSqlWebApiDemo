using System;
using FreeSqlDemo.Bussiness.Entities;
using FreeSqlDemo.Infrastructure.Repository;

namespace FreeSqlDemo.Bussiness.Repository
{
    public class RoleRepository : UnitOfWorkRepository<Role, int>, IRoleRepository
    {
        public RoleRepository(IServiceProvider service) : base(service)
        {
        }
    }
}
