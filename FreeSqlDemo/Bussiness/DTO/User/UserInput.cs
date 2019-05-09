using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSqlDemo.Domain.Entities;

namespace FreeSqlDemo.Bussiness.DTO.User
{
    public class UserInput
    {
        public string RealName { get; set; }
        public string Account { get; set; }
        public Gender Gender { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int TerantId { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public virtual ICollection<int> UserRoles { get; set; }
    }
}
