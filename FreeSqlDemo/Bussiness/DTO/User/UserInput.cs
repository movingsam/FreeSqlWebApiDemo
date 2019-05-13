using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FreeSqlDemo.Bussiness.Entities;

namespace FreeSqlDemo.Bussiness.DTO.User
{
    public class UserInput
    {
        [Required]
        public string RealName { get; set; }
        [Required]
        public string Account { get; set; }
        public Gender Gender { get; set; } = Gender.Male;
        [Required]
        public string Password { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        //[Required]
        public int TerantId { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public virtual ICollection<int> UserRoles { get; set; }
    }
}
