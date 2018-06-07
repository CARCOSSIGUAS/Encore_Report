using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belcorp.Encore.Services.Report.Controllers
{
    [Produces("application/json")]
    [Route("api/language")]
    public class LanguageController : Controller
    {
        private readonly ITermTranslationsService TermTranslationsService;

        public LanguageController(ITermTranslationsService _TermTranslationsService)
        {
            TermTranslationsService = _TermTranslationsService;
        }

        [HttpGet("{LanguageId}", Name = "GetLanguage")]
        public IActionResult GetLanguage(int languageID, string country)
        {
            var result = TermTranslationsService.GetLanguage(languageID, country);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}