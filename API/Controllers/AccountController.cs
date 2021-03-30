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
        private IAccountService _accountService;
        public AccountController(IAccountDetailService accountDetailService, IAccountService accountService)
        {
            _accountDetailService = accountDetailService;
            _accountService = accountService;


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
        [HttpPut]
        [Route("LockAccount")]
        public IActionResult LockAccount(string Id)
        {
            try
            {
                this._accountService.LockAccount(Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
    }
}
