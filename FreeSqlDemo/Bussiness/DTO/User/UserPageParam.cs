using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSqlDemo.Infrastructure.Entity;
using FreeSqlDemo.Infrastructure.Entity.Page;

namespace FreeSqlDemo.Bussiness.DTO.User
{
    public class UserPageParam : PageBase
    {
        public string KeyWord { get; set; }
        public List<int> RoleIds { get; set; }
        public bool? IsDelete { get; set; }
    }
}
