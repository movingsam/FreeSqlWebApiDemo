using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FreeSqlDemo.Bussiness.DTO.Login
{
    public class LoginDto
    {
        [Required]
        public string Account { get; set; }
        [Required]
        public string PassWord { get; set; }
    }
}
