using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;

namespace Belcorp.Encore.Services.Report.Filters
{
    public class FilterActionProxy : ActionFilterAttribute
    {

        private readonly string _dnsName;

        public FilterActionProxy(IConfiguration configuration)
        {
            _dnsName = configuration.GetSection("Encore:DnsBRA").Value;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var demo = context.HttpContext.Request.Host.Value;
            var country = "USA";

            if (demo.IndexOf(_dnsName) >0)
            {
                country = "BRA";
            }

            context.ActionArguments["country"] = country;
        }
    }
}
