using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Belcorp.Encore.Services.Report.Filters
{
    public class FilterActionProxy : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var demo = context.HttpContext.Request.Host.Value;
            var country = "USA";
            if (demo.IndexOf("5001")>0)
            {
                country = "BRA";
            }
            context.ActionArguments["country"] = country;
        }
    }
}
