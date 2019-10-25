using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using State.Or.Oya.Jjis.StatusMonitor.BackgroundServices;
using State.Or.Oya.Jjis.StatusMonitor.Util;
using State.Or.Oya.StatusMonitor.Client.Generated;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   internal class Program
   {
      static IConfiguration _config = new ConfigurationBuilder()
         .AddJsonFile("appsettings.json", true, true)
         .Build();

      private static async Task Main(string[] args)
      {
         await new HostBuilder()
            .ConfigureServices((context, services) => services.ConfigureServices(_config))
            .RunConsoleAsync();

         Console.WriteLine("Application Exiting... ");
      }
   }

   internal static class ServiceCollectionExtension
   {
      internal static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration config)
      {
         services
            .AddHostedService<StatusMonitorBackgroundService>()
            .AddHostedService<ConfigurationUpdaterBackgroundService>()
            .AddLogging(builder =>
            {
               builder.AddConsole();
               builder.SetMinimumLevel(LogLevel.Information);
            })
            .AddSingleton<INetworkUtil, NetworkUtil>()
            .AddSingleton<StatusMonitorConfiguration>()
            .AddHttpClient<StatusMonitorClient>(client => client.BaseAddress = new Uri(config["ApiUrl"])); ;

         return services;
      }
   }
}