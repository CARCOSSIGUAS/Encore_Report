﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Belcorp.Encore.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Belcorp.Encore.Services.Report.Controllers
{
    [Produces("application/json")]
    [Route("api/security")]
    public class SecurityController : Controller
    {
        private readonly IAccountsService accountsService;

        public SecurityController(IAccountsService _accountsService)
        {
            accountsService = _accountsService;
        }

        [HttpGet("singlesignon")]
        public async Task<IActionResult> SingleSignOn(string token, string country)
        {
            var result = await accountsService.GetAccountFromSingleSignOnToken(token, country);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}