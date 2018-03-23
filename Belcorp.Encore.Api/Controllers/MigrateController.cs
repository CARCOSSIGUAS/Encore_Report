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

        public MigrateController(IAccountInformationService _accountInformationService)
        {
            accountInformationService = _accountInformationService;
        }

        [HttpGet("[action]/{PeriodId}")]
        public void Report_Period(int periodId)
        {
            BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByPeriod(periodId));
        }
    }
}