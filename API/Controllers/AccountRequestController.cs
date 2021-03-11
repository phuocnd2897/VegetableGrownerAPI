using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VG.Model.Model;
using VG.Service.Service;

namespace API.Controllers
{
    [Authorize("Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountRequestController : ControllerBase
    {
        private IAccountRequestService _accountRequestService;
        public AccountRequestController(IAccountRequestService accountRequestService)
        {
            _accountRequestService = accountRequestService;
        }
        [HttpPost]
        public IActionResult SendRequestFriend(AccountRequest newItem)
        {
            try
            {
                var result = this._accountRequestService.Add(newItem);
                if (result == null)
                {
                    return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
        [HttpPut]
        public IActionResult IsConfirm(int Id, int status)
        {
            try
            {
                 this._accountRequestService.IsComfirm(Id, status);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
    }
}
