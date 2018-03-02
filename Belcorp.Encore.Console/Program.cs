using System;
using System.Collections.Generic;
using System.Text;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Application;
using Microsoft.Extensions.DependencyInjection;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Data.Contexts;
using Microsoft.Extensions.Configuration;
using Belcorp.Encore.Console.InstanceProviders;
using System.IO;

namespace Belcorp.Encore.Console
{
    class Program
    {
        private static IConfigurationRoot Configuration;

        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var provider = services.BuildServiceProvider();

            IAccountInformationService _accountInformationService = provider.GetService<IAccountInformationService>();
            var list = _accountInformationService.GetListAccountInformationByPeriodId(201210);

            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            System.Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services)
        {

            services
                .AddDbContext<EncoreCommissions_Context>(opt => opt.UseSqlServer("Data Source=10.12.6.187;Initial Catalog=BelcorpBRACommissions;User ID=usrencorebrasilqas;Password=Belcorp2016%"))
                .AddUnitOfWork<EncoreCommissions_Context>();

            services.RegisterServices();
        }
    }
}
