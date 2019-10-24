using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using State.Or.Oya.Jjis.StatusMonitor.BackgroundServices;
using State.Or.Oya.Jjis.StatusMonitor.Util;
using System;
using System.Threading.Tasks;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   internal class Program
   {
      private static async Task Main(string[] args)
      {
         await new HostBuilder()
            .ConfigureServices((context, services) => services.ConfigureServices())
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
            .AddHostedService<ConfigurationUpdaterBackgroundService>()
            .AddLogging(builder =>
            {
               builder.AddConsole();
               builder.SetMinimumLevel(LogLevel.Information);
            })
            .AddTransient<IStatusMonitorApi, FakeStatusMonitorApi>()
            .AddSingleton<INetworkUtil, NetworkUtil>()
            .AddSingleton(sp => StatusMonitorConfigurationFactory.Create(sp.GetService<IStatusMonitorApi>(), sp.GetService<INetworkUtil>()));
            

         return services;
      }
   }
}