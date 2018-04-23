using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Application.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belcorp.Encore.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Migration")]
    public class MigrationController : Controller
    {
        private readonly IAccountInformationService accountInformationService;
        private readonly IAccountsService accountsService;
        private readonly IPeriodsService periodsService;
        private readonly IMonitorMongoService monitorMongoService;

        public MigrationController
        (
            IAccountInformationService _accountInformationService,
            IAccountsService _accountsService,
            IPeriodsService _periodsService,
            IMonitorMongoService _monitorMongoService
        )
        {
            accountInformationService = _accountInformationService;
            accountsService = _accountsService;
            periodsService = _periodsService;
            monitorMongoService = _monitorMongoService;
        }

        [HttpGet("[action]/{PeriodId}")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult AccountsInformation(int periodId)
        {
            BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByPeriod(periodId));
            return Json(new { Status = "Processing Background" } );
        }

        [HttpGet("[action]")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult Accounts()
        {
            BackgroundJob.Enqueue(() => accountsService.Migrate_Accounts());
            return Json(new { Status = "Processing Background" });
        }

        [HttpGet("[action]")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult Periods()
        {
            BackgroundJob.Enqueue(() => periodsService.Migrate_Periods());
            return Json(new { Status = "Processing Background" });
        }
    }
}