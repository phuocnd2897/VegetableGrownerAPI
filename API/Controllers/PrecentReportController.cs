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
    public class PrecentReportController : ControllerBase
    {

        private IPrecentReportService _precentReportService;
        public PrecentReportController(IPrecentReportService precentReportService)
        {
            _precentReportService = precentReportService;
        }
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                var result = this._precentReportService.GetAll();
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại!");
            }
        }
        [HttpGet]
        public IActionResult Get(int Id)
        {
            try
            {
                var result = this._precentReportService.Get(Id);
                if (result != null)
                    return Ok(result);
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại!");
            }
            catch (Exception)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại!");
            }
        }
        [HttpPut]
        public IActionResult UpdatePrecent(PrecentReport newItem)
        {
            try
            {
                var result = this._precentReportService.UpdatePrecent(newItem);
                if (result != null)
                    return Ok(result);
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại!");
            }
            catch (Exception)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại!");
            }
        }
    }
}
