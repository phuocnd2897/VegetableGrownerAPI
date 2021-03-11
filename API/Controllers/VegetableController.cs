using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class VegetableController : ControllerBase
    {
        private IVegetableService _vegetableService;
        private IVegetableImageService _vegetableImageService;

        public VegetableController(IVegetableService vegetableService, IVegetableImageService vegetableImageService)
        {
            _vegetableService = vegetableService;
            _vegetableImageService = vegetableImageService;
        }
        [HttpPost]
        public IActionResult AddVegetable(VegetableRequestModel newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var baseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
                var result = this._vegetableService.Add(newItem, phoneNumber, Directory.GetCurrentDirectory(), baseUrl);
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
        [HttpPost]
        [Route("Image")]
        public IActionResult AddImageVegetable(IFormFile newItem)
        {
            try
            {
                var baseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
                var phoneNumber = User.Identity.Name;
                var result = this._vegetableImageService.UploadImage(newItem, "f8974e7d-91f9-4230-92af-9004b2d7a0a5", Directory.GetCurrentDirectory(), baseUrl);
                if (result == null)
                {
                    return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
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
        public IActionResult DeleteVegetable(int noVeg, int gardenId)
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
