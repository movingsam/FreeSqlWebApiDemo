using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;
using MySqlX.XDevAPI.Relational;

namespace FreeSqlDemo.Infrastructure.Entity
{
    public class EntityBase<TKey>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public TKey Id { get; set; }
        /// <summary>
        /// 租户ID (演示多租户功能使用)
        /// </summary>
        public int TerantId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
