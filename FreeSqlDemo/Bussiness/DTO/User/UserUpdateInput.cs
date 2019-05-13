using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FreeSqlDemo.Bussiness.Entities;

namespace FreeSqlDemo.Bussiness.DTO.User
{
    /// <summary>
    /// 用户更新模型
    /// </summary>
    public class UserUpdateInput
    {
        [Required]
        public string RealName { get; set; }
        public Gender Gender { get; set; } = Gender.Male;
        [Required]
        public string Password { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public virtual ICollection<int> UserRoles { get; set; }
    }
}
