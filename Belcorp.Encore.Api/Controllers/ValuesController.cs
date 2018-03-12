﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Application;
using Belcorp.Encore.Application.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Belcorp.Encore.Api.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IAccountInformationService accountInformationService;
        private readonly IAccountsService accountsServices;

        public ValuesController(IAccountInformationService _accountInformationService, IAccountsService _accountsServices)   
        {
            accountInformationService = _accountInformationService;
            accountsServices = _accountsServices;
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
            BackgroundJob.Enqueue(() => accountsServices.Migrate_Accounts());
            BackgroundJob.Enqueue(() => accountInformationService.Migrate_AccountInformationByPeriod(201701));
            return "Wait";


            #region Pruebas_Background
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
            #endregion
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
