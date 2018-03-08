﻿using System;
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
            id = 2015;
            id = id * 100;

            //Job background en Paralelo.
            //for (int i = 1; i <= 12; i++)
            //{
            //    BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByPeriod(id + i));
            //}

            //Job background en Continuacion uno despues de otro.
            //string jobParent = "";
            //for (int i = 1; i <= 12; i++)
            //{
            //    if (i == 1)
            //    {
            //      jobParent = BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByPeriod(id + i));
            //    }
            //    else
            //      jobParent = BackgroundJob.ContinueWith(jobParent, () => accountInformationService.Migrate_AccountInformationByPeriod(id + i));
            //}

            //var jobParent = BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByPeriod(id));
            //BackgroundJob.ContinueWith(jobParent, () => accountInformationService.Migrate_AccountInformationByPeriod(id));

            //RecurringJob.AddOrUpdate(() => accountInformationService.Migrate_AccountInformationByPeriod(id), Cron.Daily(00, 00), TimeZoneInfo.Local);
            accountInformationService.Migrate_AccountInformationByAccountId(201712, 560934);

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
