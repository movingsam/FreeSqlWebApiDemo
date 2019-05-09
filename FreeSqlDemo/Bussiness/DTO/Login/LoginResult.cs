using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FreeSqlDemo.Bussiness.DTO.Login
{
    public class LoginResult
    {
        public string Subject { get; set; }
        public LoginCode Code { get; set; } = LoginCode.NoUser;
        //public ICollection<Claim> Claims { get; set; } = new List<Claim>();
        public string AccessToken { get; set; }
    }

    public enum LoginCode
    {
        NoUser = 204,
        PasswordError = 400,
        Success = 200
    }
}
