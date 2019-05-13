using FreeSqlDemo.Bussiness.DTO.Login;
using FreeSqlDemo.Bussiness.DTO.Role;
using FreeSqlDemo.Bussiness.DTO.Terant;
using FreeSqlDemo.Bussiness.DTO.User;
using FreeSqlDemo.Bussiness.Service;
using FreeSqlDemo.Infrastructure.MVC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeSqlDemo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : DemoControllerBase
    {
        private readonly ICommonService _commonService;
        public CommonController(IServiceProvider service) : base(service)
        {
            _commonService = service.GetRequiredService<ICommonService>();
        }

        // POST api/Common/Terant
        [HttpPost("Terant")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateTerant([FromBody]TerantInput input)
        {
            return Ok(await _commonService.AddTerant(input));
        }

        // GET api/Common/User
        [HttpPost("User")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody]UserInput input)
        {
            return Ok(await _commonService.AddUser(input));
        }

        // POST api/Common/Login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm]LoginDto dto)
        {
            var res = await _commonService.Login(dto);
            return Ok(res);
        }
        // DELETE api/values/5
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            var res = await _commonService.LogOut();
            return Ok(res);
        }
        // Get api/Common/User/Page
        [HttpGet("User/Page")]
        public async Task<IActionResult> GetUserPage([FromQuery]UserPageParam param)
        {
            return Ok(await _commonService.GetUserPageAsync(param));
        }


        [HttpPut("User/Id/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateInput input)
        {
            return Ok(await _commonService.UpdateUser(id, input));
        }

        [HttpPut("User/Disable")]
        public async Task<IActionResult> UserDisable([FromBody]List<int> ids)
        {
            return Ok(await _commonService.DisableUser(ids.ToArray()));
        }

        [HttpPost("Role")]
        public async Task<IActionResult> AddRole([FromBody]RoleInput input)
        {
            return Ok(await _commonService.AddRole(input));
        }
        [HttpGet("Role/Page")]
        public async Task<IActionResult> GetRolePage([FromQuery]RolePageParam param)
        {
            return Ok(await _commonService.GetRolePageAsync(param));
        }
        [HttpPut("Role/Id/{id}")]
        public async Task<IActionResult> UpdateRolePage(int id, [FromBody] RoleUpdateInput input)
        {
            return Ok(await _commonService.RoleUpdate(id, input));
        }

        [HttpPut("Role/Disable")]
        public async Task<IActionResult> RoleDisable([FromBody]List<int> ids)
        {
            return Ok(await _commonService.DisableRole(ids));
        }

    }
}
