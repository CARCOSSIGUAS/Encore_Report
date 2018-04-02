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
    [Route("api/OnlineMLM")]
    public class OnlineMLMController : Controller
    {
        private readonly IProcessOnlineMlmService processOnlineMlmService;

        public OnlineMLMController(IProcessOnlineMlmService _processOnlineMlmService)
        {
            processOnlineMlmService = _processOnlineMlmService;
        }

        [HttpGet("Process/{orderId}")]
        [AutomaticRetry(Attempts = 0)]
        public ActionResult Process(int orderId)
        {
            BackgroundJob.Enqueue(() => processOnlineMlmService.ProcessMLM(orderId));
            return Json(new { Status = "Processing Background" });
        }

    }
}