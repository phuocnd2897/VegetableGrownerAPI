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
    public class WardController : ControllerBase
    {
        private IWardService _wardService;
        public WardController(IWardService wardService)
        {
            _wardService = wardService;
        }
        [HttpGet]
        public IActionResult GetByDistrictId(int Id)
        {
            try
            {
                var result = this._wardService.GetByDistrictId(Id);
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
