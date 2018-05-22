using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belcorp.Encore.Services.Report.Controllers
{
    [Produces("application/json")]
    [Route("api/reportperformance")]
	public class ReportPerformanceController : Controller
    {
		private readonly IReportPerformanceService reportPerformanceService;

		public ReportPerformanceController(IReportPerformanceService _reportPerformanceService)
		{
            reportPerformanceService = _reportPerformanceService;
		}

        // GET: api/reportperformance
        [HttpGet("[action]")]
        public async Task<IActionResult> GetPerformance_Detail(int accountId, int periodId)
        {
            var header = await reportPerformanceService.GetPerformance_Detail(accountId, periodId);
            return Json(header);
        }

        // GET: api/reportperformance
        [HttpGet("[action]")]
        public JsonResult GetPerformance_AccountInformation(int accountId, int periodId)
        {
            var header = reportPerformanceService.GetPerformance_AccountInformation(accountId, periodId);
            return Json(header);
        }
    }
}
