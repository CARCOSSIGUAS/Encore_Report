using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Application.Extension;
using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Services.Report.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belcorp.Encore.Services.Report.Controllers
{
    [Produces("application/json")]
    [Route("api/reportperformance")]
    [ServiceFilter(typeof(FilterActionProxy))]
    public class ReportPerformanceController : Controller
    {
		private readonly IReportPerformanceService reportPerformanceService;

		public ReportPerformanceController(IReportPerformanceService _reportPerformanceService)
		{
            reportPerformanceService = _reportPerformanceService;
		}


        [HttpGet("getPerformanceByAccount", Name = "GetPerformanceByAccount")]
        public async Task<IActionResult> GetStatesByPeriods(int accountID, int periodID, string country = null)
        {
            var result = await reportPerformanceService.GetPerformanceByAccount(accountID, periodID, country);
            var item = result.ToReportAccountPerformance_DTO(country);
            var retorno = await reportPerformanceService.GetRequirements(item, country);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(retorno);
        }

        [HttpGet("getPerformanceBySponsor", Name = "GetPerformanceBySponsor")]
       public async Task<IActionResult> GetPerformanceBySponsor(int sponsorID, int periodID, string country = null)
        {
            var result = await reportPerformanceService.GetPerformanceBySponsor(sponsorID, periodID, country);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result.ToReportAccountPerformance_DTO());
        }


        // GET: api/reportperformance
        [HttpGet("[action]")]
        public async Task<IActionResult> GetPerformance_Detail(int accountId, int periodId, string country = null)
        {
            var header = await reportPerformanceService.GetPerformance_Detail(accountId, periodId, country);
            return Json(header);
        }

    }
}
