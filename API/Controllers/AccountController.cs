﻿using System;
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
        [HttpGet]
        [Route("GetAccountDetailByPhoneNumber")]
        public IActionResult GetAccountDetailByPhoneNumber(string Id)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._accountDetailService.GetAccountDetailById(Id);
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
        [HttpGet]
        [Route("GetPassword")]
        public IActionResult GetPassword(string phoneNumber)
        {
            try
            {
                var result = this._accountService.GetAccountPassword(phoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Route("ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordRequestModel newItem)
        {
            try
            {
                this._accountService.ChangePassword(newItem);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
