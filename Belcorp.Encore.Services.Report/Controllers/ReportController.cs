using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Entities.Entities.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Belcorp.Encore.Services.Report.Controllers
{
	[Produces("application/json")]
	[Route("api/report")]
	public class ReportController : Controller
	{
		private readonly IAccountInformationService accountInformationService;
		private readonly IAccountsService accountsService;
		private readonly IMonitorMongoService monitorMongoService;

		public ReportController(IAccountInformationService _accountInformationService, IAccountsService _accountsService, IMonitorMongoService _monitorMongoService)
		{
			accountInformationService = _accountInformationService;
			accountsService = _accountsService;
			monitorMongoService = _monitorMongoService;
		}

		[HttpGet("[action]")]
		public async Task<ActionResult> Accounts(int accountId)
		{
			var result = await accountsService.GetListAccounts(accountId);

			if (result == null)
			{
				NotFound();
			}

			return Ok(result);
		}

		// GET: api/Report
		[HttpGet("[action]")]
		public async Task<IActionResult> GetPerformance_Header(int accountId, int periodId)
		{
			var header = await accountInformationService.GetPerformance_Header(accountId, periodId);

			return Json(header);
		}

		// GET: api/Report
		[HttpGet("[action]")]
		public async Task<IActionResult> GetPerformance_Detail(int accountId, int periodId)
		{
			var header = await accountInformationService.GetPerformance_Detail(accountId, periodId);

			return Json(header);
		}

		[HttpGet("[action]")]
		public IActionResult GetAccountsFilterPaginated(Filtros_DTO filtrosDTO)
		{
			if (filtrosDTO == null)
			{
				return BadRequest();
			}

			var header = accountInformationService.GetAccounts(filtrosDTO);

			return Json(header);
		}

		// GET: api/Report
		[HttpGet("[action]")]
		public JsonResult GetPerformance_HeaderFront(int accountId, int periodId)
		{
			var header = accountInformationService.GetPerformance_HeaderFront(accountId, periodId);
			return Json(header);
		}
		// GET: api/Report
		[HttpGet("[action]")]
		public JsonResult GetPerformance_AccountInformation(int accountId, int periodId)
		{
			var header = accountInformationService.GetPerformance_AccountInformation(accountId, periodId);
			return Json(header);
		}

        [HttpGet("exportexcel")]
        public async Task<IActionResult> Exportexcel()
        {
            const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var result = await accountsService.GetListAccounts(1697);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Excel");
                worksheet.Cells["A1"].LoadFromCollection(result, PrintHeaders: true);

                int noOfProperties = result.GetType().GetGenericArguments()[0].GetProperties().Length;

                using (ExcelRange r = worksheet.Cells[1, 1, 1, noOfProperties])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                }

                for (var col = 1; col < noOfProperties + 1; col++)
                {
                    worksheet.Column(col).AutoFit();
                }

                return File(package.GetAsByteArray(), XlsxContentType, "result_excel.xlsx");
            }
        }
    }
}

