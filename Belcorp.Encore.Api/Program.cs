using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Belcorp.Encore.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseUrls("http://localhost:9050") //BRA
                .UseUrls("http://localhost:9023") //USAC:\Users\user\Downloads\Encore_Report\Encore_Report\Belcorp.Encore.Api\Controllers\OnlineMlmController.cs
                .UseStartup<Startup>()
                .Build();
    }
}
