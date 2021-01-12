using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VG.Model.RequestModel;
using VG.Service.Service;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountDetailService _accountDetailService;
        public AccountController(IAccountDetailService accountDetailService)
        {
            _accountDetailService = accountDetailService;
        
        }
        [HttpPost]
        [Route("Registration")]
        public IActionResult RegistrationAccount(AccountRequestModel newItem)
        {
            try
            {
                var account = this._accountDetailService.RegistrationAccount(newItem);
                if (account == null)
                {
                    return BadRequest("Có lỗi xảy ra vui lòng thử lại");
                }
                return Ok(account);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Route("Update")]
        public IActionResult UpdateAccount(AccountRequestModel newItem)
        {
            try
            {
                var account = this._accountDetailService.UpdateAccountDetail(newItem);
                if (account == null)
                {
                    return BadRequest("Có lỗi xảy ra vui lòng thử lại");
                }
                return Ok(account);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
