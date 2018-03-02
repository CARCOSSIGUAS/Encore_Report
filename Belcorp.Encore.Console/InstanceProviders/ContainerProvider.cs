
namespace Belcorp.Encore.Console.InstanceProviders
{
    using Belcorp.Encore.Application;
    using Belcorp.Encore.Repositories;

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

        }
    }
}