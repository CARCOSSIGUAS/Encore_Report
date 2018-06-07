using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Api.Filters;
using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Application.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belcorp.Encore.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/onlinemlm")]
    public class OnlineMLMController : Controller
    {
        private readonly IProcessOnlineMlmService processOnlineMlmService;

        public OnlineMLMController(IProcessOnlineMlmService _processOnlineMlmService)
        {
            processOnlineMlmService = _processOnlineMlmService;
        }

        [HttpGet("orders/{orderId}")]
        [AutomaticRetry(Attempts = 0)]
        [ServiceFilter(typeof(FilterActionProxy))]
        public ActionResult Orders(int orderId, string country)
        {
            BackgroundJob.Enqueue(() => processOnlineMlmService.ProcessMLMOrder(orderId, country));
            return Json(new { Status = "Processing Background" });
        }

        [HttpGet("monitorlotes/{loteId}")]
        [AutomaticRetry(Attempts = 0)]
        [ServiceFilter(typeof(FilterActionProxy))]
        public ActionResult MonitorLotes(int loteId, string country)
        {
            BackgroundJob.Enqueue(() => processOnlineMlmService.ProcessMLMLote(loteId, country));
            return Json(new { Status = "Processing Background" });
        }

    }
}