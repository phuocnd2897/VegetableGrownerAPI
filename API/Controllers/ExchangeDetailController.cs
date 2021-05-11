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
        public IActionResult AddExchange(ExchangeDetailRequestModel newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var baseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
                var result = this._exchangeDetailService.Add(newItem, phoneNumber, baseUrl);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                    return BadRequest("Số lượng còn lại không đủ. Vui lòng nhập lại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        [Route("CheckInstance")]
        public IActionResult CheckInstance(ExchangeDetailRequestModel newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var baseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
                var result = this._exchangeDetailService.CheckInstance(newItem, phoneNumber, baseUrl);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
        [HttpGet]
        [Route("GetExchangeRequest")]
        public IActionResult GetExchangeRequest()
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._exchangeDetailService.GetExchangeRequest(phoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
        [HttpPut]
        public IActionResult UpdateExchangeDetail(ExchangeDetailRequestModel newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._exchangeDetailService.Update(newItem);
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
        [HttpPut]
        [Route("IsAccept")]
        public IActionResult AcceptExchangeDetail(string Id, int Status)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var baseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
                this._exchangeDetailService.IsAccept(Id, Status, baseUrl);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Route("Finish")]
        public IActionResult FinishExchangeDetail(string Id)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var baseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
                this._exchangeDetailService.Finish(Id, phoneNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
