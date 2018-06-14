﻿using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Services.Report.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Belcorp.Encore.Services.Report.Controllers
{
    [Produces("application/json")]
    [Route("api/home")]
    [ServiceFilter(typeof(FilterActionProxy))]
    public class HomeController : Controller
    {
        private readonly IHomeService homeService;

        public HomeController (IHomeService _homeService)
        {
            homeService = _homeService;
        }

        [HttpGet("header/{accountId}", Name = "GetHeader")]
        public IActionResult GetHeader(int accountId, string country = null)
        {
            var contextCountry = country;
            var result = homeService.GetHeader(accountId, country);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("performanceindicator", Name = "GetPerformanceIndicator")]
        public IActionResult GetPerformanceIndicator(int accountId, string country = null)
        {
            var result = homeService.GetPerformanceIndicator(accountId, country);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("kpisIndicator", Name = "GetKpisIndicator")]
        public IActionResult GetkpisIndicator(int periodID, int SponsorID, int DownLineID, string country = null)
        {
            var result = homeService.GetkpisIndicator(periodID, SponsorID, DownLineID, country);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("bonusIndicator", Name = "GetBonusIndicator")]
        public IActionResult GetBonusIndicator(int SponsorID, int periodID, string country = null)
        {
            var result = homeService.GetBonusIndicator(SponsorID, periodID, country);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}