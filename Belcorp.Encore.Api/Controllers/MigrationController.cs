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
        public ActionResult AccountsInformation(int periodId)
        {
            BackgroundJob.Enqueue(() => migrateService.MigrateAccountInformationByPeriod(periodId));
            return Json(new { Status = "Processing Background" } );
        }

        [HttpGet("accounts")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult Accounts()
        {
            BackgroundJob.Enqueue(() => migrateService.MigrateAccounts());
            return Json(new { Status = "Processing Background" });
        }

        [HttpGet("periods")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult Periods()
        {
            BackgroundJob.Enqueue(() => migrateService.MigratePeriods());
            return Json(new { Status = "Processing Background" });
        }
    }
}