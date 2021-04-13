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
        public IActionResult AddVegetable([FromForm] VegetableRequestModel newItem)
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
        //[HttpPost]
        //[Route("Image")]
        //public IActionResult AddImageVegetable(IFormFile newItem)
        //{
        //    try
        //    {
        //        var baseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
        //        var phoneNumber = User.Identity.Name;
        //        var result = this._vegetableImageService.UploadImage(newItem, "f8974e7d-91f9-4230-92af-9004b2d7a0a5", Directory.GetCurrentDirectory(), baseUrl);
        //        if (result == null)
        //        {
        //            return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpPut]
        public IActionResult UpdateVegetable(VegetableRequestModel newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var baseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);
                var result = this._vegetableService.Update(newItem, phoneNumber, Directory.GetCurrentDirectory(), baseUrl);
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
        public IActionResult DeleteVegetable(string Id)
        {
            try
            {
                this._vegetableService.Delete(Id);
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetByGardenId")]
        public IActionResult GetAll(int GardenId)
        {
            try
            {
                var result = this._vegetableService.GetByGardenId(GardenId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra vui lòng thử lại");
                throw;
            }
        }
        [HttpGet]
        [Route("SearchByDescription")]
        public IActionResult SearchByDescription(string searchValue)
        {
            try
            {
                var result = this._vegetableService.SearchByDescription(searchValue);
                return Ok(result.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra vui lòng thử lại");
            }
        }
        [HttpGet]
        [Route("SearchByName")]
        public IActionResult SearchByName(string searchValue)
        {
            try
            {
                var result = this._vegetableService.SearchByName(searchValue);
                return Ok(result.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra vui lòng thử lại");
            }
        }
        [HttpGet]
        [Route("SearchByKeyword")]
        public IActionResult SearchByKeyword(string searchValue)
        {
            try
            {
                var result = this._vegetableService.SearchByKeyword(searchValue);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra vui lòng thử lại");
            }
        }
        [HttpGet]
        [Route("SearchVegetableSharedByName")]
        public IActionResult SearchVegetableSharedByName(string searchValue)
        {
            try
            {
                var result = this._vegetableService.SearchVegetableSharedByName(searchValue);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra vui lòng thử lại");
            }
        }
        [HttpGet]
        [Route("SearchVegetableSharedByKeyword")]
        public IActionResult SearchVegetableSharedByKeyword(string searchValue)
        {
            try
            {
                var result = this._vegetableService.SearchVegetableSharedByKeyword(searchValue);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("CheckVegetableInGarden")]
        public IActionResult CheckVegetableInGarden(string Id, string Name)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._vegetableService.CheckVegetableInGarden(Id, Name, phoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetAllVegetable")]
        public IActionResult GetAllVegetable()
        {
            try
            {
                var result = this._vegetableService.GetAllVegetable();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
