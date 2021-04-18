using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VG.Service.Service;

namespace API.Controllers
{
    [Authorize("Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountFriendController : Controller
    {
        public IAccountFriendService _accountFriendService;
        public AccountFriendController(IAccountFriendService accountFriendService)
        {
            _accountFriendService = accountFriendService;
        }
        [HttpGet]
        public IActionResult GetAllFriend()
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._accountFriendService.GetAllFriend(phoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
        [HttpPut]
        public IActionResult Unfriend(int Id)
        {
            try
            {
                this._accountFriendService.Unfriend(Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
    }
}
