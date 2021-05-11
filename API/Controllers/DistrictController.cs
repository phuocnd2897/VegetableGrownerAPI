using Microsoft.AspNetCore.Authorization;
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
    public class DistrictController : ControllerBase
    {
        private IDistrictService _districtService;
        public DistrictController(IDistrictService districtService)
        {
            _districtService = districtService;
        }
        [HttpGet]
        public IActionResult GetByProvinceId(int Id)
        {
            try
            {
                var result = this._districtService.GetByProvinceId(Id);
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest("Có lỗi xảy ra vui lòng thử lại!");
            }
            catch (Exception)
            {
                return BadRequest("Có lỗi xảy ra vui lòng thử lại!");
            }
        }
    }
}
