using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ApplicationStatusMonitor.Controllers;
using AspNet.Security.ApiKey.Providers;
using AspNet.Security.ApiKey.Providers.Events;
using AspNet.Security.ApiKey.Providers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag;


namespace ApplicationStatusMonitor
{

   public class Startup
   {
      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
         ValidKeys = ( Configuration["ApiKeySettings:ApiKeys"] ?? throw new Exception("Config value for ApiKeySettings:ApiKeys is missing"))
            .Split(",").ToArray();
      }

      public string[] ValidKeys { get; set; }

      public IConfiguration Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddSwaggerDocument(config => config.PostProcess = document =>
         {
            document.Info.Version = "v1.0.0";
            document.Info.Title = "Status Monitor API";
            document.Info.Description = "An API to record status updates and manage configuration for status monitors";
            document.Info.Contact = new OpenApiContact()
            {
               Name = "Oregon Youth Authority"
            };
         });

         services.AddControllers();
         services.AddMvcCore().AddRazorViewEngine();
         services.Configure<StatusMonitorDatabaseSettings>( Configuration.GetSection(nameof(StatusMonitorDatabaseSettings)));
         services.AddSingleton<IStatusMonitorDatabaseSettings>( sp => sp.GetRequiredService<IOptions<StatusMonitorDatabaseSettings>>().Value);
         
         services.AddTransient<IStatusRepository<StatusMonitorReply>, StatusMonitorMongoRepository<StatusMonitorReply>>();
         services.AddTransient<IMonitorConfigurationRepository<MonitorConfiguration>, MonitorConfigurationMongoRepository<MonitorConfiguration>>();
         
         services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = ApiKeyDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = ApiKeyDefaults.AuthenticationScheme;
                })
                .AddApiKey(options =>
                {
                    options.Header = "X-API-KEY";
                    options.HeaderKey = String.Empty;
                    options.Events = new ApiKeyEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var ex = context.Exception;

                            Trace.TraceError(ex.Message);

                            context.Fail(ex);

                            return Task.CompletedTask;
                        },
                        OnApiKeyValidated = context =>
                        {
                           if (!ValidKeys.Contains(context.ApiKey))
                              return Task.CompletedTask;

                           context.Success();

                           return Task.CompletedTask;
                        }
                    };
                });
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }

         //app.UseHttpsRedirection();

         app.UseOpenApi();
         app.UseSwaggerUi3();

         app.UseRouting();

         app.UseAuthentication();
         app.UseAuthorization();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapDefaultControllerRoute();
         });
      }
   }
}