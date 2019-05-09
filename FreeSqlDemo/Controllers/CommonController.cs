using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FreeSqlDemo.Bussiness.DTO.Login;
using FreeSqlDemo.Bussiness.DTO.Terant;
using FreeSqlDemo.Bussiness.DTO.User;
using FreeSqlDemo.Bussiness.Service;
using FreeSqlDemo.Infrastructure.JWTOptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace FreeSqlDemo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICommonService _commonService;
        public CommonController(IServiceProvider service)
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

        // POST api/values
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm]LoginDto dto)
        {
            var res = await _commonService.Login(dto);
            var id = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
            id.AddClaim(new Claim(ClaimsType.Subject, res.Subject));
          //  await HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
            return Ok(res);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
