using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;

namespace FreeSqlDemo.Domain.Entities
{
    /// <summary>
    /// 这里不指定表名 看一下效果
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// 这里是中间表所以需要双主键
        /// </summary>
        [Column(IsPrimary = true)]
        public int UserId { get; set; }
        /// <summary>
        /// 这里是中间表所以需要双主键
        /// </summary>
        [Column(IsPrimary = true)]
        public int RoleId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
