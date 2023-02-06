using Autofac;
using LS_ERP.API.Helpers;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LS_ERP.BusinessLogic.AutofacModule;
using Microsoft.AspNetCore.Http.Features;
using Hangfire;
using Hangfire.SqlServer;
using LS_ERP.BusinessLogic.Shared.Process.Jobs;

namespace LS_ERP.API
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
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LS_ERP.API", Version = "v1" });
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.MultipartBoundaryLengthLimit = int.MaxValue;
                options.MultipartHeadersCountLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
                options.MemoryBufferThreshold = int.MaxValue;
            });

            services.AddDbContexts<AppDbContext>(Configuration);

            // Add Hangfire services.
            services.AddHangfire(o => o
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac here. Don't
            // call builder.Populate(), that happens in AutofacServiceProviderFactory
            // for you.
            builder.RegisterModule(new ApplicationAutofacModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LS_ERP.API"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<UpdatePurchaseOrderTotalAmount>(j => j.Execute(), Cron.Daily(0, 0), TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate<UpdateSizeSortIndexMasterBOMJob>(j => j.Execute(), Cron.Daily(1, 0), TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate<CreateItemMasterJob>(j => j.Execute(), Cron.Hourly(), TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate<UpdateStorageDetailJob>(j => j.Execute(), Cron.Hourly(), TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate<UpdateFabricPurchaseOrderFromTransactionsJob>(j => j.Execute(), Cron.Hourly(), TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate<UpdateItemMasterIDStorageDetailJob>(j => j.Execute(), Cron.Hourly(), TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate<UpdatePONumberScanResultJob>(j => j.Execute(), Cron.Hourly(), TimeZoneInfo.Local);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });
        }
    }
}
