using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using FreeSqlDemo.Infrastructure.Entity;
using FreeSqlDemo.Infrastructure.Entity.Page;

namespace FreeSqlDemo.Bussiness.DTO.Role
{
    public class RolePageParam : PageBase
    {
        public string KeyWord { get; set; }
        public bool? IsDelete { get; set; }
    }


}
