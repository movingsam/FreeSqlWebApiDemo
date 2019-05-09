using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSqlDemo.Infrastructure.Entity;

namespace FreeSqlDemo.Infrastructure.DomainBase
{
    public class CurrentUser
    {
        public string Account { get; set; }
        public string RealName { get; set; }
        public long PhoneNumber { get; set; }
        public ICollection<RoleVO> Roles { get; set; }
        public Terant Terant { get; set; }
    }

    public class RoleVO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
    }
}
