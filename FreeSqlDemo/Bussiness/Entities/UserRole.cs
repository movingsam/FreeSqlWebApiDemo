using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeSqlDemo.Domain.Entities
{
    /// <summary>
    /// 这里不指定 看一下效果
    /// </summary>
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
