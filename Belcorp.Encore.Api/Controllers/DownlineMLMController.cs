using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belcorp.Encore.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/DownlineMLM")]
    public class DownlineMLMController : Controller
    {
        [HttpGet]
        public void Get(int orderID, int orderStatusID)
        {

        }

    }
}