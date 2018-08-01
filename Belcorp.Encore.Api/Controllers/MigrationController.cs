using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Api.Filters;
using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Application.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belcorp.Encore.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/migration")]
    [ServiceFilter(typeof(FilterActionProxy))]
    public class MigrationController : Controller
    {
        private readonly IMigrateService migrateService;
        private readonly IMonitorMongoService monitorMongoService;

        public MigrationController
        (
            IMigrateService _migrateService,
            IMonitorMongoService _monitorMongoService
        )
        {
            migrateService = _migrateService;
            monitorMongoService = _monitorMongoService;
        }

        [HttpGet("accountsinformation/{periodId}")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult AccountsInformation(int periodId, string country = null)
        {
            BackgroundJob.Enqueue(() => migrateService.MigrateAccountInformationByPeriod(periodId, country));
            return Json(new { Status = "Processing Background" } );
        }

        [HttpGet("bonusdetails/{periodId}")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult BonusDetails(int? periodId = null, string country = null)
        {
            BackgroundJob.Enqueue(() => migrateService.MigrateBonusDetailsByPeriod(periodId, country));
            return Json(new { Status = "Processing Background" });
        }

        [HttpGet("accounts")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult Accounts(string country = null)
        {
            BackgroundJob.Enqueue(() => migrateService.MigrateAccounts(country));
            return Json(new { Status = "Processing Background" });
        }

        [HttpGet("periods")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult Periods(string country)
        {
            BackgroundJob.Enqueue(() => migrateService.MigratePeriods(country));
            return Json(new { Status = "Processing Background" });
        }

        [HttpGet("termtranslations")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult TermTranslations(string country = null)
        {
            BackgroundJob.Enqueue(() => migrateService.MigrateTermTranslations(country));
            return Json(new { Status = "Processing Background" });
        }

        [HttpGet("accountkpisdetails/{periodId}")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult AccountKPIsDetails(int? periodId = null, string country = null)
        {
            BackgroundJob.Enqueue(() => migrateService.MigrateAccountKPIsDetailsByPeriod(periodId, country));
            return Json(new { Status = "Processing Background" });
        }

        [HttpGet("requirementtitlecalculations")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult RequirementTitleCalculations(string country = null)
        {
            BackgroundJob.Enqueue(() => migrateService.RequirementTitleCalculations(country));
            return Json(new { Status = "Processing Background" });
        }

        [HttpGet("requirementlegs")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult RequirementLegs(string country = null)
        {
            BackgroundJob.Enqueue(() => migrateService.RequirementLegs(country));
            return Json(new { Status = "Processing Background" });
        }

        [HttpGet("performance/{periodId}")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult Performance(int? periodId = null, string country = null)
        {
            BackgroundJob.Enqueue(() => migrateService.Performance(country, periodId));
            return Json(new { Status = "Processing Background" });
        }


    }
}