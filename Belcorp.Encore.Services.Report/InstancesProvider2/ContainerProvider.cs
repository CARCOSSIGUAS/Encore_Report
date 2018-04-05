﻿using Belcorp.Encore.Application;
using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Belcorp.Encore.Services.Report.InstancesProvider2
{
    public static class ContainerProvider
    {
		public static IServiceCollection RegisterServices(
		   this IServiceCollection services)
		{
			ConfigureContainer(services);
			return services;
		}

		static void ConfigureContainer(IServiceCollection services)
		{
			services.AddScoped<IAccountInformationRepository, AccountInformationRepository>();
			services.AddScoped<IAccountInformationService, AccountInformationService>();

			//services.AddScoped<ISponsorTreeService, SponsorTreeService>();
			//services.AddScoped<ISponsorTreeRepository, SponsorTreeRepository>();

			services.AddScoped<IAccountsRepository, AccountsRepository>();
			services.AddScoped<IAccountsService, AccountsService>();

			//services.AddScoped<ITitlesRepository, TitlesRepository>();
			//services.AddScoped<IAccountKPIsRepository, AccountKPIsRepository>();

			//services.AddScoped<IProcessOnlineMlmService, ProcessOnlineMlmService>();
			//services.AddScoped<IProcessOnlineRepository, ProcessOnlineRepository>();
		}

	}
}
