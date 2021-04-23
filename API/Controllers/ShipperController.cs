using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VG.Service.Service;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipperController : ControllerBase
    {
        private IExchangeDetailService _exchangeDetailService;
        public ShipperController(IExchangeDetailService exchangeDetailService)
        {
            _exchangeDetailService = exchangeDetailService;
        }
        [HttpGet]
        [Route("GetInformationExchange")]
        public IActionResult GetInformationExchange(string Id)
        {
            try
            {
                var result = this._exchangeDetailService.GetInformationExchange(Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
    }
}
