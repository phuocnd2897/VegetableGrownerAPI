using Microsoft.AspNetCore.Authorization;
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
    public class VegetableController : Controller
    {
        private IVegetableService _vegetableService;
        public VegetableController(IVegetableService vegetableService)
        {
            _vegetableService = vegetableService;
        }
        [HttpPost]
        public IActionResult AddVegetable(VegetableRequestModel newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._vegetableService.Add(newItem, phoneNumber, Directory.GetCurrentDirectory());
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
        [HttpPut]
        public IActionResult UpdateVegetable(VegetableRequestModel newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._vegetableService.Update(newItem, phoneNumber, Directory.GetCurrentDirectory());
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
        [HttpDelete]
        public IActionResult AddVegetable(int noVeg, int gardenId)
        {
            try
            {
                this._vegetableService.Delete(noVeg, gardenId);
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
        [HttpGet]
        public IActionResult Get(int noVeg, int gardenId)
        {
            try
            {
                var result = this._vegetableService.Get(noVeg, gardenId);
                if (result == null)
                {
                    return BadRequest("Có lỗi xảy ra vui lòng thử lại");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra vui lòng thử lại");
                throw;
            }
        }
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                var result = this._vegetableService.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra vui lòng thử lại");
                throw;
            }
        }
    }
}
