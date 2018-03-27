using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Application.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belcorp.Encore.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Migrate")]
    public class MigrateController : Controller
    {
        private readonly IAccountInformationService accountInformationService;
        private readonly IAccountsService accountsService;

        public MigrateController(IAccountInformationService _accountInformationService, IAccountsService _accountsService)
        {
            accountInformationService = _accountInformationService;
            accountsService = _accountsService;
        }

        [HttpGet("[action]/{PeriodId}")]
        public ActionResult Report_Period(int periodId)
        {
            BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByPeriod());
            return Json(new { Status = "Process" });
        }

        [HttpGet("[action]")]
        public ActionResult Report_Period()
        {
            BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByPeriod());
            return Json(new { Status = "Process" } );
        }

        [HttpGet("[action]")]
        public ActionResult Migrate_Accounts()
        {
            accountsService.Migrate_Accounts();
            return Json(new { Status = "Process" });
        }
    }
}