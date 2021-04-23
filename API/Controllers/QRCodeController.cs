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
    public class QRCodeController : ControllerBase
    {
        private IQRCodeService _qrCodeService;
        private IQRCodeForShipperService _qrCodeForShipperService;
        public QRCodeController(IQRCodeService qrCodeService, IQRCodeForShipperService qrCodeForShipperService)
        {
            _qrCodeService = qrCodeService;
            _qrCodeForShipperService = qrCodeForShipperService;
        }
        [HttpPost]
        public IActionResult CreateQRCode(string ExchangeId)
        {
            try
            {
                var baseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
                var result = this._qrCodeForShipperService.Add(ExchangeId, baseUrl);
                if (result == null)
                {
                    return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
                }
                else return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public IActionResult GetQRCodeForShipper(string ExchangeId)
        {
            try
            {
                var result = this._qrCodeForShipperService.Get(ExchangeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
