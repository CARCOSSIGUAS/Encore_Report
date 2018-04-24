using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Services.Report.InstancesProvider2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Belcorp.Encore.Services.Report
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

			services
			.AddDbContext<EncoreCommissions_Context>(opt => opt.UseSqlServer(Configuration.GetConnectionString("Encore_Commissions")))
			.AddDbContext<EncoreCore_Context>(opt => opt.UseSqlServer(Configuration.GetConnectionString("Encore_Core")))
			.AddUnitOfWork<EncoreCommissions_Context, EncoreCore_Context>();

            services.Configure<Settings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("Encore_Mongo:ConnectionString").Value;
                options.Database = Configuration.GetSection("Encore_Mongo:Database").Value;
            });

            services.RegisterServices();

			services.AddMvc();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
			app.UseCors(builder =>
						builder.WithOrigins("http://localhost:3000", "http://10.12.9.41", "http://10.12.9.83:3392")
                        .AllowAnyHeader());

			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }   

            app.UseMvc();
        }
    }
}
