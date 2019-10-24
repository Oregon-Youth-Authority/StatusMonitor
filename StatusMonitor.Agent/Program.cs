using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using State.Or.Oya.Jjis.StatusMonitor.Monitors;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   class Program
   {
      static async Task Main(string[] args)
      {
         await new HostBuilder()
            .ConfigureServices((context, services) =>  services.ConfigureServices())
            .RunConsoleAsync();
         
         Console.WriteLine("Application Exiting... ");
      }
   }

   internal static class ServiceCollectionExtension
   {
      internal static IServiceCollection ConfigureServices(this IServiceCollection services)
      {
         services
            .AddHostedService<StatusMonitorBackgroundService>()
            .AddLogging(builder =>
            {
               builder.AddConsole();
               builder.SetMinimumLevel(LogLevel.Information);
            })
            .AddSingleton<StatusMonitorConfiguration>(sp => new StatusMonitorConfiguration
            {
               Monitors = new List<IStatusMonitor>
               {
                  new PortStatusStatusMonitor("DB Connectivity", "", 0),
                  new PortStatusStatusMonitor("Web Connectivity", "", 0)
               }
            });

         return services;
      }
   }
}
