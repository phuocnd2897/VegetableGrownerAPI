using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VG.Common.Constant;
using VG.Model.RequestModel;
using VG.Service.Service;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IAccountService _accountService;
        public LoginController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("MobileLogin")]
        public IActionResult MobileLogin([FromBody] LoginRequestModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ!" });
            var user = this._accountService.Login(login.PhoneNumber, login.Password);
            if (user == null || user.RoleId != 1)
                return BadRequest(new { message = "Tên đăng nhập hoặc mật khẩu không đúng!" });
            //var roles = this._appUserService.getrolesbyuserid(user.UserId);
            //var claimroles = roles.SelectMany(s => s.Functions).GroupBy(s => s.FunctionCode, (k, g) => new CacheRoleFunctionsModel()
            //{
            //    FunctionCode = k,
            //    Roles = g.SelectMany(t => t.Roles).GroupBy(t => t, (k1, g1) => k1).ToArray()
            //}).ToList();
            //var rolename = this._appuserService.getrolename(user.UserId);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppConst.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.PhoneNumber.ToString()),
                    new Claim("AccountId",user.AccountId)
                }),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            //user.Roles = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(roles)));
            return Ok(user);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("WebLogin")]
        public IActionResult WebLogin([FromBody] LoginRequestModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ!" });
            var user = this._accountService.Login(login.PhoneNumber, login.Password);
            if (user == null && (user.RoleId != 0))
                return BadRequest(new { message = "Tên đăng nhập hoặc mật khẩu không đúng!" });
            //var roles = this._appUserService.getrolesbyuserid(user.UserId);
            //var claimroles = roles.SelectMany(s => s.Functions).GroupBy(s => s.FunctionCode, (k, g) => new CacheRoleFunctionsModel()
            //{
            //    FunctionCode = k,
            //    Roles = g.SelectMany(t => t.Roles).GroupBy(t => t, (k1, g1) => k1).ToArray()
            //}).ToList();
            //var rolename = this._appuserService.getrolename(user.UserId);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppConst.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.PhoneNumber.ToString()),
                    new Claim("AccountId",user.AccountId)
                }),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            //user.Roles = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(roles)));
            return Ok(user);
        }
    }
}
