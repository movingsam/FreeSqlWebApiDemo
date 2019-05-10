using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSqlDemo.Bussiness.DTO.User;

namespace FreeSqlDemo.Bussiness.DTO.Role
{
    /// <summary>
    /// 角色ViewObject
    /// </summary>
    public class RoleVO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public int UserCount => User?.Count ?? 0;
        public List<UserVO> User { get; set; }
    }
}
