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
    [Route("api/OnlineMlm")]
    public class OnlineMlmController : Controller
    {
        private readonly IProcessOnlineMlmService processOnlineMlmService;

        public OnlineMlmController(IProcessOnlineMlmService _processOnlineMlmService)
        {
            processOnlineMlmService = _processOnlineMlmService;
        }

        [HttpGet("[action]/{orderId}")]
        [AutomaticRetry(Attempts = 0)]
        public void Process(int orderId)
        {
            BackgroundJob.Enqueue(() => processOnlineMlmService.ProcessMLM(orderId));
        }

    }
}