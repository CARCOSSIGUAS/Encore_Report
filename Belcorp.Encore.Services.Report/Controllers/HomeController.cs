using Belcorp.Encore.Application.Extension;
using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Entities.Entities.Search;
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

        [HttpGet("getTransactionDate", Name = "GetTransactionDate")]
        public IActionResult GetTransactionDate(string country = null)
        {
            var contextCountry = country;
            var result = homeService.GetLastTransaction(country);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("performanceindicator", Name = "GetPerformanceIndicator")]
        public IActionResult GetPerformanceIndicator(int accountId, int? periodID, string country = null)
        {
            var result = homeService.GetPerformanceIndicator(accountId, periodID, country);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("kpisindicator", Name = "GetKpisIndicator")]
        public IActionResult GetkpisIndicator(int periodID, int SponsorID, int DownLineID, string country = null)
        {
            var result = homeService.GetKpisIndicator(periodID, SponsorID, DownLineID, country);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("bonusindicator", Name = "GetBonusIndicator")]
        public IActionResult GetBonusIndicator(int SponsorID,string country = null)
        {
            var result = homeService.GetBonusIndicator(SponsorID,  country);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("searchconsultant", Name = "GetConsultant")]
        public IActionResult GetConsultant(string filter, int accountID, string country = null)
        {
            var result = homeService.GetConsultantSearch(filter, accountID, country);

            if (result == null)
            {
                return NotFound();
            }
            return Json(new { items = result.toAccountsAutocomplete_DTO(), word = filter });
        }

        [HttpGet("newsindicator", Name = "GetNewsIndicator")]
        public IActionResult GetNewsIndicator(int accountID, int periodoID, string country = null)
        {
            var result = homeService.GetNewsIndicator(accountID, periodoID, country);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("consultantlower", Name = "GetConsultantLowerPerformance")]
        public IActionResult GetConsultantLowerPerformance(int? periodID, int accountID, string country = null)
        {
            var result = homeService.GetConsultantLowerPerformance(periodID, accountID, country);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result.ToReportAccount_DTO());
        }
    }
}