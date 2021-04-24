using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VG.Model.RequestModel;
using VG.Service.Service;

namespace API.Controllers
{
    [Authorize("Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountDetailController : ControllerBase
    {
        private IAccountDetailService _accountDetailService;
        private IAccountService _accountService;
        public AccountDetailController(IAccountDetailService accountDetailService, IAccountService accountService)
        {
            _accountDetailService = accountDetailService;
            _accountService = accountService;
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
        [Route("UploadAvata")]
        public IActionResult UploadAvata(IFormFile newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var baseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
                this._accountDetailService.UploadAvatar(newItem, Directory.GetCurrentDirectory(), baseUrl, phoneNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Route("ChangePassword")]
        public IActionResult ChangePassword(string newPass)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                this._accountDetailService.ChangePassword(newPass, phoneNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetAccountDetailByPhoneNumber")]
        public IActionResult GetAccountDetailByPhoneNumber(string Id)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._accountDetailService.GetAccountDetailById(Id, phoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
        [HttpGet]
        [Route("SearchAccount")]
        public IActionResult SearchAccount(string searchValue)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._accountDetailService.SearchAccount(searchValue);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
    }
}
