using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationStatusMonitor.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
      }

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
         
         services.Configure<StatusMonitorDatabaseSettings>( Configuration.GetSection(nameof(StatusMonitorDatabaseSettings)));
         services.AddSingleton<IStatusMonitorDatabaseSettings>( sp => sp.GetRequiredService<IOptions<StatusMonitorDatabaseSettings>>().Value);

         services.AddTransient<IStatusRepository<StatusMonitorReply>, StatusMonitorMongoRepository<StatusMonitorReply>>();
         services.AddTransient<IMonitorConfigurationRepository<MonitorConfiguration>, MonitorConfigurationMongoRepository<MonitorConfiguration>>();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }

         //app.UseHttpsRedirection();

         // Enable middleware to serve generated Swagger as a JSON endpoint.
         app.UseOpenApi();
         app.UseSwaggerUi3();

         app.UseRouting();

         //app.UseAuthorization();

         
         app.UseEndpoints(endpoints =>
         {
            endpoints.MapDefaultControllerRoute();
         });
      }
   }
}