using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Application;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Belcorp.Encore.Api.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IAccountInformationService accountInformationService;
        public ValuesController(IAccountInformationService _accountInformationService)   
        {
            accountInformationService = _accountInformationService;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            accountInformationService.Migrate_AccountInformationByPeriod(id);
            
            //BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByPeriod(id));
            //BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByAccountId(id));
            //BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByAccountId(id));
            //BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByAccountId(id));

            //var jobParent = BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByAccountId(id));
            //BackgroundJob.ContinueWith(jobParent, () => accountInformationService.Migrate_AccountInformationByAccountId(id));

            //RecurringJob.AddOrUpdate(() => accountInformationService.Migrate_AccountInformationByAccountId(id), Cron.Daily(11, 15), TimeZoneInfo.Local);
            return "Wait";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
