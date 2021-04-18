using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VG.Service.Service;

namespace API.Controllers
{
    [ApiController]
    public class WikiController : ControllerBase
    {
        private IWikiService _wikiService;
        public WikiController(IWikiService wikiService)
        {
            _wikiService = wikiService;
        }
        [HttpGet]
        [Route("LeakInfoFromWiki")]
        public IActionResult leakInfoFromWiki(string title)
        {
            try
            {
                var result = this._wikiService.LeakInfoFromWikiByTitle(title);
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
        [HttpGet]
        [Route("GetDescription")]
        public IActionResult GetDescription(string title)
        {
            try
            {
                var result = this._wikiService.GetDescription(title);
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
    }
}
