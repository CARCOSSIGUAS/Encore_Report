namespace Belcorp.Encore.Api.InstanceProviders
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
            services.AddScoped<IAccountInformationRepository, AccountInformationRepository>();
            services.AddScoped<IAccountInformationService, AccountInformationService>();

            services.AddScoped<ISponsorTreeService, SponsorTreeService>();
            services.AddScoped<ISponsorTreeRepository, SponsorTreeRepository>();

            services.AddScoped<IAccountsRepository, AccountsRepository>();
            services.AddScoped<IAccountsService, AccountsService>();

            services.AddScoped<ITitlesRepository, TitlesRepository>();
            services.AddScoped<IAccountKPIsRepository, AccountKPIsRepository>();

            services.AddScoped<IProcessOnlineMlmService, ProcessOnlineMlmService>();
            services.AddScoped<IProcessOnlineRepository, ProcessOnlineRepository>();

            services.AddScoped<IMonitorMongoService, MonitorService>();
            services.AddScoped<IMonitorRepository, MonitorRepository>();

        }
    }
}