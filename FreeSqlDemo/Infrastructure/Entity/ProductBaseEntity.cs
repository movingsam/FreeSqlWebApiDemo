using System;

namespace FreeSqlDemo.Infrastructure.Entity
{
    public abstract class ProductBaseEntity : EntityBase<string>
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 更新用户Id
        /// </summary>
        public int UpdateUserId { get; set; }
        /// <summary>
        /// 创建用户Id
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// 创建用户名
        /// </summary>
        public string CreateUserName { get; set; }
        /// <summary>
        /// 更新用户名
        /// </summary>
        public string UpdateUserName { get; set; }
        public int LogId { get; set; }
        public LogEntity Log { get; set; }
    }
}
