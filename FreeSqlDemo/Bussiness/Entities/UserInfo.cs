using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;
using FreeSqlDemo.Infrastructure.Entity;

namespace FreeSqlDemo.Bussiness.Entities
{
    [Table(Name = "Base_UserInfo")]
    public class UserInfo : EntityBase<int>
    {
        /// <summary>
        /// 导航属性
        /// </summary>
        public User User { get; set; }
        [Column(DbType = "nvarchar(15)")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
