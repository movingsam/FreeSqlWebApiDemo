using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSqlDemo.Domain.Entities;
using FreeSqlDemo.Infrastructure.DomainBase;

namespace FreeSqlDemo.Bussiness.DTO.User
{
    public class UserVO
    {
        public int Id { get; set; }
        public string RealName { get; set; }
        public string Account { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public ICollection<CurrentRole> Roles { get; set; }
    }
}
