using ExchangeRateAPI.Database;
using ExchangeRateAPI.Services.Helpers;
using ExchangeRateAPI.Services.Helpers.Interfaces;
using ExchangeRateAPI.Services.Manager;
using ExchangeRateAPI.Services.Manager.Interfaces;
using ExchangeRateAPI.Services.Repositories;
using ExchangeRateAPI.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI
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
            AddScopeds(services);

            services.AddHttpClient();

            services.AddControllers()
                    .AddNewtonsoftJson();

            services.AddSwaggerGen(c => { 
                    c.SwaggerDoc("v1", 
                                new OpenApiInfo { 
                                        Title = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, 
                                        Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() 
                                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddScopeds(IServiceCollection services)
        {
            var conn = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ExchangeDBContext>(options => options.UseSqlServer(conn));

            #region [ HELPERS ]

            services.AddScoped<IEcbApiHandler, EcbApiHandler>();

            #endregion

            #region [ MANAGERS ]

            services.AddScoped<ISqlClient, SqlClient>();

            #endregion

            #region [ REPOSITORIES ]

            services.AddScoped<IApiKeysRepository, ApiKeysRepository>();
            services.AddScoped<IExchangeRatesRepository, ExchangeRatesRepository>();

            #endregion
        }
    }
}
