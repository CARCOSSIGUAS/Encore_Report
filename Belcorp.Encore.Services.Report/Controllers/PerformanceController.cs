using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belcorp.Encore.Services.Report.Controllers
{
    [Produces("application/json")]
    [Route("api/Performance")]
	public class PerformanceController : Controller
    {
		private readonly IAccountInformationService accountInformationService;
		private readonly IAccountsService accountsService;

		public PerformanceController(IAccountInformationService _accountInformationService, IAccountsService _accountsService)
		{
			accountInformationService = _accountInformationService;
			accountsService = _accountsService;
		}

		// GET: api/Performance
		[HttpGet("[action]/{AccountId}/{PeriodId}")]
		public async Task<IActionResult> GetPerformance_Header(int accountId,int periodId)
        {
			var header = await accountInformationService.GetPerformance_Header(accountId, periodId);

			return Json(header);
        }

		// GET: api/Performance
		[HttpGet("[action]/{AccountId}/{PeriodId}")]
		public async Task<IActionResult> GetPerformance_Detail(int accountId, int periodId)
		{
			var header = await accountInformationService.GetPerformance_Detail(accountId, periodId);

			return Json(header);
		}


		// GET: api/Performance/5
		[HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Performance
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Performance/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
