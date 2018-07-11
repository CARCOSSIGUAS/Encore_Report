using Belcorp.Encore.Api.Filters;
using Belcorp.Encore.Api.InstanceProviders;
using Belcorp.Encore.Application;
using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Belcorp.Encore.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = Configuration.GetSection("Encore:ConnectionStringPrincipal").Value;

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<FilterActionProxy>();


            services.AddHangfire(config =>
            {
                // Read DefaultConnection string from appsettings.json
                var connectionString = Configuration.GetSection("Encore_Mongo:" + connection).Value;
                var databaseName = Configuration.GetSection("Encore_Mongo:Database_HangFire").Value + "_" + Configuration.GetSection("Encore:Country").Value;

                var storageOptions = new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        Strategy = MongoMigrationStrategy.Migrate,
                        BackupStrategy = MongoBackupStrategy.Collections
                    }
                };
                config.UseMongoStorage(connectionString, databaseName, storageOptions);
            });

            services.AddMvc(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
            });

            services
            .AddDbContext<EncoreCommissions_Context>(opt => opt.UseSqlServer(Configuration.GetConnectionString("Encore_Commissions")))
            .AddDbContext<EncoreCore_Context>(opt => opt.UseSqlServer(Configuration.GetConnectionString("Encore_Core")))
            .AddUnitOfWork<EncoreCommissions_Context, EncoreCore_Context>();

            #region Mongo
            services.Configure<Settings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("Encore_Mongo:" + connection).Value;
                options.Database = Configuration.GetSection("Encore_Mongo:Database").Value + Configuration.GetSection("Encore:Country").Value;
            });
            #endregion

            services.RegisterServices();

            #region HangFire_Jobs

            JobStorage.Current = new MongoStorage(Configuration.GetSection("Encore_Mongo:" + connection).Value, Configuration.GetSection("Encore_Mongo:Database_HangFire").Value + "_" + Configuration.GetSection("Encore:Country").Value);
            var provider = services.BuildServiceProvider();
            IMigrateService migrateService = provider.GetService<IMigrateService>();
            IMonitorMongoService monitorMongoService = provider.GetService<IMonitorMongoService>();
            IProcessOnlineMlmService processOnlineMlmService = provider.GetService<IProcessOnlineMlmService>();

            //Todos los dias a las xxx:xx
            RecurringJob.AddOrUpdate("Monitor_CloseDaily_Information", () => migrateService.MigrateAccountInformationByPeriod(null, Configuration.GetSection("Encore:Country").Value), Configuration.GetSection("Encore:ScheduleTask").Value);
            RecurringJob.AddOrUpdate("Monitor_CloseDaily_Bonus", () => migrateService.MigrateBonusDetailsByPeriod(null, Configuration.GetSection("Encore:Country").Value), Configuration.GetSection("Encore:ScheduleTask").Value);
            RecurringJob.AddOrUpdate("Monitor_CloseDaily_Kpis", () => migrateService.MigrateAccountKPIsDetailsByPeriod(null, Configuration.GetSection("Encore:Country").Value), Configuration.GetSection("Encore:ScheduleTask").Value);

            //Todos los dias, cada xx minutos
            RecurringJob.AddOrUpdate("Monitor_Migrate", () => monitorMongoService.Migrate(Configuration.GetSection("Encore:Country").Value), Cron.MinuteInterval(int.Parse(Configuration.GetSection("Encore:ScheduleTaskDaily").Value)));
            #endregion

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                HeartbeatInterval = new System.TimeSpan(0, 5, 0),
                ServerCheckInterval = new System.TimeSpan(0, 5, 0),
                SchedulePollingInterval = new System.TimeSpan(0, 1, 0)
            });

            app.UseHangfireDashboard("/Monitor");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
