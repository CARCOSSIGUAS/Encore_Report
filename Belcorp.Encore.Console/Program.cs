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
using System.Linq;
using System.IO;
using Belcorp.Encore.Application.Services;

namespace Belcorp.Encore.Console
{
    class Program
    {
        private static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var provider = services.BuildServiceProvider();

            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();


            System.Console.WriteLine($"Iniciando lectura...");
            IAccountInformationService accountInformationService = provider.GetService<IAccountInformationService>();
            var list = accountInformationService.GetListAccountInformationByPeriodId(201512).ToList();
            System.Console.WriteLine($"Total de registros: {list.Count}");
            System.Console.WriteLine($"Fin lectura...");
            System.Console.WriteLine();


            System.Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services)
        {

            services
                .AddDbContext<EncoreCommissions_Context>(opt => opt.UseSqlServer(configuration.GetConnectionString("Encore_Commissions")))
                .AddUnitOfWork<EncoreCommissions_Context>();

            services.RegisterServices();
        }
    }
}
