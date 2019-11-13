using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using State.Or.Oya.Jjis.StatusMonitor.BackgroundServices;
using State.Or.Oya.Jjis.StatusMonitor.Configuration;
using State.Or.Oya.Jjis.StatusMonitor.Util;
using State.Or.Oya.Jjis.StatusMonitor.WindowsServices;
using State.Or.Oya.StatusMonitor.Client.Generated;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   internal class Program
   {
      private static async Task Main(string[] args)
      {
         var isService = !(Debugger.IsAttached || args.All(arg => arg != "--service"));

         var config = GetConfiguration();
         var hostBuilder = new HostBuilder().ConfigureServices((context, services) => services.ConfigureServices(config));
         if (isService)
            await hostBuilder.RunAsServiceAsync();
         else
            await hostBuilder.RunConsoleAsync();

         Console.WriteLine("Application Exiting... ");
      }

      internal static IConfiguration GetConfiguration()
      {
         var appSettingsFilename = "appsettings.json";
         var apiKeySettingsFilename = "apiKey.json";
         var monitorSettingsFilename = "monitorSettings.json";

         string ConfigFallback((string currentDir, string fallBack) fallbackTuple) => File.Exists(fallbackTuple.currentDir) ? fallbackTuple.currentDir : fallbackTuple.fallBack;

         var applicationDirectory = GetApplicationDirectory();
         var appSettingsFile = ConfigFallback((Path.Combine(applicationDirectory, appSettingsFilename), appSettingsFilename));
         var apiKeyFile = ConfigFallback((Path.Combine(applicationDirectory, apiKeySettingsFilename), apiKeySettingsFilename));
         var monitorSettingsFile = ConfigFallback((Path.Combine(applicationDirectory, monitorSettingsFilename), monitorSettingsFilename));

         return new ConfigurationBuilder()
            .AddJsonFile(appSettingsFile, false, true)
            .AddJsonFile(apiKeyFile, optional: false)
            .AddJsonFile(monitorSettingsFile, true, false)
            .Build();
      }

      internal static string GetApplicationDirectory()
      {
         var appDirectory = Environment.CurrentDirectory;
         try
         {
            var processName = !Debugger.IsAttached
               ? "JJISStatusMonitorAgent"
               : "JJISStatusMonitorAgentWorker";
            
            var process = Process.GetProcessesByName(processName)?.FirstOrDefault();
            
            var processPath = process?.MainModule?.FileName;
            return processPath != null
               ? new FileInfo(processPath).DirectoryName
               : appDirectory;
         }
         catch { }

         return appDirectory;
      }
   }

   internal static class ServiceCollectionExtension
   {
      internal static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration config)
      {
         services
            .Configure<ApiKeySettings>(config.GetSection(nameof(ApiKeySettings)))
            .AddHostedService<ConfigurationUpdaterBackgroundService>()
            .AddHostedService<StatusMonitorBackgroundService>()
            .AddLogging(builder =>
            {
               builder
                  .AddEventLog(settings => settings.Filter = (s, level) => level >= LogLevel.Warning)
                  .AddConsole()
                  .SetMinimumLevel(LogLevel.Information);
            })
            .AddTransient<ApplicationConfiguration>()
            .AddTransient(sp => config)
            .AddSingleton<MonitorSettings>()
            .AddSingleton<INetworkUtil, NetworkUtil>()
            .AddSingleton<StatusMonitorConfiguration>()
            .AddSingleton(sp => new StatusMonitorClientFactory(sp))
            .AddTransient<ApiKeyHandler>()
            .AddHttpClient<StatusMonitorClient>(client => client.BaseAddress = new Uri(config["ApiUrl"]))
            .AddHttpMessageHandler<ApiKeyHandler>();

         return services;
      }
   }
}