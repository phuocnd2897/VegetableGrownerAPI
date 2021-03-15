using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VG.Data.Repository;
using VG.Model.RequestModel;
using VG.Service.Service;

namespace API.Controllers
{
    [Authorize("Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeDetailController : ControllerBase
    {
        private IExchangeDetailService _exchangeDetailService;
        public ExchangeDetailController(IExchangeDetailService exchangeDetailService)
        {
            _exchangeDetailService = exchangeDetailService;
        }
        [HttpPost]
        public IActionResult AddShare(ExchangeDetailRequestModel newItem)
        {
            try
            {
                var result = this._exchangeDetailService.Add(newItem);
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
                var result = this._exchangeDetailService.Get(Id);
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
        [Route("GetByAccountId")]
        public IActionResult GetExchangeDetailByAccountId()
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._exchangeDetailService.GetByAccountId(phoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
        [HttpPut]
        public IActionResult UpdateShareDetail(ExchangeDetailRequestModel newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._exchangeDetailService.Update(newItem);
                if (result == null)
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
                this._exchangeDetailService.Delete(Id);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
    }
}
