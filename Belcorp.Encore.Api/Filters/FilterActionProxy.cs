using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Belcorp.Encore.Api.Filters
{
    public class FilterActionProxy : ActionFilterAttribute
    {
        private readonly string _country;

        public FilterActionProxy(IConfiguration configuration)
        {
            _country = configuration.GetSection("Encore:Country").Value;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.ActionArguments["country"] = _country;
        }
    }
}
