using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Entities.Entities.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belcorp.Encore.Services.Report.Controllers
{
	[Produces("application/json")]
	[Route("api/Report")]
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

		[HttpPost("[action]")]
		public JsonResult Mundo(int periodo)
		{
			var header = "Hola";

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
		public JsonResult GetPerformance_HeaderFront(int accountId, int period)
		{
			var header = accountInformationService.GetPerformance_HeaderFront(accountId, period);
			return Json(header);
		}
		// GET: api/Report
		[HttpGet("[action]")]
		public JsonResult GetPerformance_AccountInformation(int accountId, int periodId)
		{
			var header = accountInformationService.GetPerformance_AccountInformation(accountId, periodId);
			return Json(header);
		}
	}
}