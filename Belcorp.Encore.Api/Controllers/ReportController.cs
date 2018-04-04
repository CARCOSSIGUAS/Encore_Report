using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belcorp.Encore.Api.Controllers
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

        [HttpGet("[action]/{accountId}")]
        public async Task<ActionResult> Accounts(int accountId)
        {
            var result = await accountsService.GetListAccounts(accountId);

            if (result == null)
            {
                NotFound();
            }

            return Ok(result);
        }
    }
}