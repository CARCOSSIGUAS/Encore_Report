using Belcorp.Encore.Api.InstanceProviders;
using Belcorp.Encore.Data.Contexts;
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
            services.AddHangfire(config =>
            {
                // Read DefaultConnection string from appsettings.json
                var connectionString = Configuration.GetConnectionString("Encore_Mongo");

                var storageOptions = new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        Strategy = MongoMigrationStrategy.Migrate,
                        BackupStrategy = MongoBackupStrategy.Collections
                    }
                };
                config.UseMongoStorage(connectionString, "Encore_HangFire", storageOptions);
            });

            services
            .AddDbContext<EncoreCommissions_Context>(opt => opt.UseSqlServer(Configuration.GetConnectionString("Encore_Commissions")))
            .AddDbContext<EncoreCore_Context>(opt => opt.UseSqlServer(Configuration.GetConnectionString("Encore_Core")))
            .AddUnitOfWork<EncoreCommissions_Context, EncoreCore_Context>();


            services.RegisterServices();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
