using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using VG.Model.Model;
using VG.Model.RequestModel;
using VG.Service.Service;

namespace API.Controllers
{
    [Authorize("Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShareDetailController : ControllerBase
    {
        private IShareDetailService _shareDetailService;
        private IWebHostEnvironment _hostingEnvironment;
        public ShareDetailController(IShareDetailService shareDetailService, IWebHostEnvironment hostingEnvironment)
        {
            _shareDetailService = shareDetailService;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost]
        public IActionResult AddShare(ShareDetailRequestModel newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._shareDetailService.Add(newItem, phoneNumber);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                    return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }

        }
        [HttpGet]
        public IActionResult Get(string Id)
        {
            try
            {
                var result = this._shareDetailService.Get(Id);
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
        [HttpGet]
        [Route("GetAllShareById")]
        public IActionResult GetAllShareById(string Id)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._shareDetailService.GetShareByAccountId(Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._shareDetailService.GetAll(phoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
        [HttpPut]
        public IActionResult UpdateShareDetail(ShareDetailRequestModel newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._shareDetailService.Update(newItem, phoneNumber, Directory.GetCurrentDirectory(), Request.Scheme + "://" + Request.Host);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                    return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
            catch (Exception)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
        [HttpDelete]
        public IActionResult Delete(string Id)
        {
            try
            {
                this._shareDetailService.Delete(Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("SearchShareByName")]
        public IActionResult SearchShareByName(string valueSearch)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._shareDetailService.SearchShareByName(valueSearch, phoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
        [HttpGet]
        [Route("SearchShareByKeyword")]
        public IActionResult SearchShareByKeyword(string valueSearch)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._shareDetailService.SearchShareByKeyword(valueSearch, phoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
        [HttpGet]
        [Route("SearchShareByDescription")]
        public IActionResult SearchShareByDescription(string valueSearch)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._shareDetailService.SearchShareByDescription(valueSearch, phoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
    }
}
