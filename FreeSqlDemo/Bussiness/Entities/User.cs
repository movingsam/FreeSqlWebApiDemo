using System.Collections.Generic;
using FreeSql.DataAnnotations;
using FreeSqlDemo.Infrastructure.Entity;

namespace FreeSqlDemo.Bussiness.Entities
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
