using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using VG.Model.Model;
using VG.Model.RequestModel;
using VG.Service.Service;

namespace API.Controllers
{
    [Authorize("Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private IPostService _postService;
        private IWebHostEnvironment _hostingEnvironment;
        public PostController(IPostService postService, IWebHostEnvironment hostingEnvironment)
        {
            _postService = postService;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost]
        public IActionResult AddPost(PostRequestModel newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._postService.Add(newItem, phoneNumber, Directory.GetCurrentDirectory(), Request.Scheme + "://" + Request.Host);
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
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAllPost()
        {
            try
            {
                var result = this._postService.GetAllPost();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
        [HttpPut]
        public IActionResult UpdatePost(PostRequestModel newItem)
        {
            try
            {
                var phoneNumber = User.Identity.Name;
                var result = this._postService.Update(newItem, phoneNumber, Directory.GetCurrentDirectory(), Request.Scheme + "://" + Request.Host);
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
        public IActionResult Delete(string postId)
        {
            try
            {
                this._postService.Delete(postId);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest("Có lỗi xảy ra. Vui lòng thử lại");
            }
        }
    }
}
