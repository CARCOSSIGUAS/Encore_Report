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

        [HttpGet("accountsInformation/{periodId}")]
        [AutomaticRetry(Attempts = 0)]
        [ServiceFilter(typeof(FilterActionProxy))]
        public ActionResult AccountsInformation(int periodId, string country = null)
        {
            BackgroundJob.Enqueue(() => migrateService.MigrateAccountInformationByPeriod(country, periodId));
            return Json(new { Status = "Processing Background" } );
        }

        [HttpGet("accounts")]
        [AutomaticRetry(Attempts = 0)]
        [ServiceFilter(typeof(FilterActionProxy))]
        public ActionResult Accounts(string country)
        {
            BackgroundJob.Enqueue(() => migrateService.MigrateAccounts(country));
            return Json(new { Status = "Processing Background" });
        }

        [HttpGet("periods")]
        [AutomaticRetry(Attempts = 0)]
        [ServiceFilter(typeof(FilterActionProxy))]
        public ActionResult Periods(string country)
        {
            BackgroundJob.Enqueue(() => migrateService.MigratePeriods(country));
            return Json(new { Status = "Processing Background" });
        }

        [HttpGet("termtranslations")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult TermTranslations()
        {
            BackgroundJob.Enqueue(() => migrateService.MigrateTermTranslations());
            return Json(new { Status = "Processing Background" });
        }
    }
}