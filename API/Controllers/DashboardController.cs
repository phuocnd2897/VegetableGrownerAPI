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
    public class DashboardController : ControllerBase
    {
        private IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet]
        [Route("GetDashboard")]
        public IActionResult Dashboard()
        {
            var result = this._dashboardService.ShowDashboard();
            return Ok(result);
        }
        [HttpGet]
        [Route("ShowDashBoardAboutShareAndExchange")]
        public IActionResult ShowDashBoardAboutShareAndExchange()
        {
            var result = this._dashboardService.ShowDashBoardAboutShareAndExchange();
            return Ok(result);
        }
        [HttpGet]
        [Route("Top10")]
        public IActionResult GetTop10(int Status)
        {
            var result = this._dashboardService.GetTop10(Status);
            return Ok(result);
        }
    }
}
