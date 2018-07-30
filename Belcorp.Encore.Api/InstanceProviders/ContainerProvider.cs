namespace Belcorp.Encore.Api.InstanceProviders
{
    using Belcorp.Encore.Application;
	using Belcorp.Encore.Application.Interfaces;
	using Belcorp.Encore.Application.Services;
    using Belcorp.Encore.Application.Services.Interfaces;
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
            services.AddScoped<IReportAccountService, ReportAccountService>();

            services.AddScoped<ISponsorTreeRepository, SponsorTreeRepository>();

            services.AddScoped<IAccountsRepository, AccountsRepository>();
            services.AddScoped<IAccountKPIsDetailsRepository, AccountsKPIDetailsRepository>();
            services.AddScoped<IBonusDetailsRepository, BonusDetailsRepository>();
            services.AddScoped<IAccountsService, AccountsService>();

            services.AddScoped<ITitlesRepository, TitlesRepository>();
            

            services.AddScoped<IProcessOnlineMlmService, ProcessOnlineMlmService>();
            services.AddScoped<IProcessOnlineRepository, ProcessOnlineRepository>();

            services.AddScoped<IMonitorMongoService, MonitorService>();
            services.AddScoped<IMonitorRepository, MonitorRepository>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<IMigrateService, MigrateService>();

            services.AddScoped<ICalculationTypesService, CalculationTypesService>();

            services.AddScoped<IOrderCalculationTypesService, OrderCalculationTypesService>();

            services.AddScoped<IPersonalIndicatorLogService, PersonalIndicatorLogService>();

            services.AddScoped<IPersonalIndicatorDetailLogService, PersonalIndicatorDetailLogService>();

            services.AddScoped<IAccountConsistencyStatusesService, AccountConsistencyStatusesService>();

            services.AddScoped<IActivityStatusesService, ActivityStatusesService>();
            
            services.AddScoped<IAccountKPIsService, AccountKPIsService>();
            services.AddScoped<IAccountKPIsRepository, AccountKPIsRepository>();

            services.AddScoped<IAccountStatusesService, AccountStatusesService>();

            services.AddScoped<IOrdersRepository, OrdersRepository>();

            services.AddScoped<IActivitiesRepository, ActivitiesRepository>();

            services.AddScoped<ISystemConfigsService, SystemConfigsService>();

        }
    }
}