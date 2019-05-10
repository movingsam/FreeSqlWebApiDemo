using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSqlDemo.Infrastructure.Entity.Page;

namespace FreeSqlDemo.Bussiness.DTO.Role
{
    public class RolePageParam : PageBase
    {
        public string KeyWord { get; set; }
    }
}
