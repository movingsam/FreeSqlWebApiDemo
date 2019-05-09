using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;
using FreeSqlDemo.Infrastructure.Entity;
using MySqlX.XDevAPI.Relational;

namespace FreeSqlDemo.Domain.Entities
{
    [Table(Name = "Base_Role")]
    public class Role : EntityBase<int>
    {
        /// <summary>
        /// 默认NVARCHAR(255)
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 修改一下Code的类型成NVarchar50
        /// </summary>
        [Column(DbType = "nvarchar(50)")]
        public string Code { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
