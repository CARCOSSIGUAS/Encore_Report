﻿namespace Belcorp.Encore.Api.InstanceProviders
{
    using Belcorp.Encore.Application;
	using Belcorp.Encore.Application.Interfaces;
	using Belcorp.Encore.Application.Services;
    using Belcorp.Encore.Repositories;
	using Belcorp.Encore.Repositories.Interfaces;
	using Belcorp.Encore.Repositories.Repositories;
	using Microsoft.Extensions.DependencyInjection;
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
            services.AddTransient<IAccountInformationRepository, AccountInformationRepository>();
            services.AddTransient<IAccountInformationService, AccountInformationService>();

            services.AddTransient<ISponsorTreeService, SponsorTreeService>();
            services.AddTransient<ISponsorTreeRepository, SponsorTreeRepository>();

            services.AddTransient<IAccountsRepository, AccountsRepository>();
            services.AddTransient<IAccountsService, AccountsService>();

            services.AddTransient<ITitlesRepository, TitlesRepository>();
            services.AddTransient<IAccountKPIsRepository, AccountKPIsRepository>();

            services.AddTransient<IProcessOnlineMlmService, ProcessOnlineMlmService>();
            services.AddTransient<IProcessOnlineRepository, ProcessOnlineRepository>();
        }
    }
}