using System;
using FreeSql.DataAnnotations;
using MySqlX.XDevAPI.Relational;

namespace FreeSqlDemo.Infrastructure.Entity
{
    /// <summary>
    /// 租户实体
    /// </summary>
    [Table(Name = "Sys_Terant")]
    public class Terant : ITerant
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public int Effective { get; set; }
        /// <summary>
        /// 忽略掉这个属性 不存入数据库 动态计算
        /// </summary>
        [Column(IsIgnore = true)]
        public DateTime ExpiredDate
        {
            get => CreateDate.AddMonths(Effective);
        }
    }
    /// <summary>
    /// 租户接口
    /// </summary>
    public interface ITerant
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime CreateDate { get; set; }
        int Effective { get; set; }
        DateTime ExpiredDate { get; }
    }
}
