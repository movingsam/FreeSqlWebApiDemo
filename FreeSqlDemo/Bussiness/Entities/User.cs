using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;
using FreeSqlDemo.Bussiness.Entities;
using FreeSqlDemo.Infrastructure.Entity;
using Microsoft.AspNetCore.Identity;
using MySqlX.XDevAPI.Relational;

namespace FreeSqlDemo.Domain.Entities
{
    /// <summary>
    /// 指定表名
    /// </summary>
    [Table(Name = "Base_User")]
    public class User : EntityBase<int>
    {
        [Column(DbType = "nvarchar(50)")]
        public string RealName { get; set; }
        [Column(DbType = "nvarchar(50)")]
        public string Account { get; set; }
        /// <summary>
        /// 这里演示一下MapType 将枚举转成string存入
        /// 当然也可以用int存入 改成typeof(int)
        /// </summary>
        [Column(MapType = typeof(string))]
        public Gender Gender { get; set; }
        /// <summary>
        /// 演示一下修改字段名
        /// </summary>
        [Column(Name = "PassWord")]
        public string Password { get; set; }
        /// <summary>
        /// 租户
        /// </summary>
        public Terant Terant { get; set; }
        public int UserInfo_Id { get; set; }
        public UserInfo UserInfo { get; set; } = new UserInfo();
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
    /// <summary>
    /// 性别
    /// </summary>
    public enum Gender
    {
        Male,
        FeMale
    }
}
