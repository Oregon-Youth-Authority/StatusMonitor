using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using State.Or.Oya.Jjis.StatusMonitor.Configuration;
using State.Or.Oya.Jjis.StatusMonitor.Monitors;
using State.Or.Oya.StatusMonitor.Client.Generated;

namespace State.Or.Oya.Jjis.StatusMonitor.BackgroundServices
{
   public class StatusMonitorBackgroundService : BackgroundService
   {
      private readonly MonitorSettings _monitorSettings;
      private readonly StatusMonitorConfiguration _statusMonitorConfiguration;
      private readonly StatusMonitorClientFactory _statusMonitorClientFactory;
      private readonly ILogger<StatusMonitorBackgroundService> _logger;
      private readonly int _statusCheckInterval;
      private readonly int _statusUpdateInterval;
      private bool _monitorsLoaded = false;

      public StatusMonitorBackgroundService(ApplicationConfiguration applicationConfiguration, MonitorSettings monitorSettings, StatusMonitorConfiguration statusMonitorConfiguration, StatusMonitorClientFactory statusMonitorClientFactory, ILogger<StatusMonitorBackgroundService> logger)
      {
         _monitorSettings = monitorSettings;
         _statusMonitorConfiguration = statusMonitorConfiguration;
         _statusMonitorClientFactory = statusMonitorClientFactory;
         _statusCheckInterval = applicationConfiguration.StatusCheckInterval;
         _statusUpdateInterval = applicationConfiguration.StatusUpdateInterval;
         _logger = logger;
      }

      protected override async Task ExecuteAsync(CancellationToken stoppingToken)
      {
         // Wait for ConfigurationUpdaterBackgroundService to get monitor configuration
         await WaitForMonitorConfiguration(stoppingToken);

         while (!stoppingToken.IsCancellationRequested)
         {
            try
            {
               foreach (var monitor in _statusMonitorConfiguration.Monitors)
               {
                  if (await monitor.HasStatusChanged())
                  {
                     await PersistMonitorStatus(monitor, stoppingToken);
                     _logger.LogInformation(
                        $"{DateTime.Now} {monitor.Name} has changed from {monitor.PreviousStatus} to {monitor.Status}");
                     continue;
                  }

                  if (monitor.LastStatusChange >= DateTime.Now.AddMilliseconds(_statusUpdateInterval * -1)) continue;
                  await PersistMonitorStatus(monitor, stoppingToken);
                  monitor.LastStatusChange = DateTime.Now;
                  _logger.LogInformation($"{DateTime.Now} {monitor.Name} is {monitor.Status}");
               }
            }
            catch (Exception ex)
            {
               _logger.LogError($"An unexpected error occurred. {ex.Message}");
            }

            await Task.Delay(_statusCheckInterval, stoppingToken);
         }
      }

      private bool IsNewConfig(string config)
      {
         return !string.IsNullOrEmpty(config) &&
                !string.Equals(config, "Detect", StringComparison.CurrentCultureIgnoreCase);
      }

      private async Task PersistMonitorStatus(IStatusMonitor monitor, CancellationToken stoppingToken)
      {
         var client = _statusMonitorClientFactory.Create();
         await client.UpdateStatusAsync(new StatusMonitorRequest
         {
            MonitorName = monitor.Name,
            Status = monitor.Status.ToString(),
            DisplayName = GetDisplayName(monitor.Name),
            LocationId = GetLocationId(monitor.Name)
         }, stoppingToken);
      }

      private string GetDisplayName(string monitorName)
      {
         var configValue = _monitorSettings.MonitorConfigurations.FirstOrDefault(mc => mc.Name == monitorName && IsNewConfig(mc.Computer))?.Computer;
         if (configValue != null)
         {
            _logger.LogInformation($"Configuration file (monitorSettings.json) overrides computer name for {monitorName} to {configValue}");
         }

         return configValue ?? Environment.MachineName;
      }

      private string GetLocationId(string monitorName)
      {
         var configValue = _monitorSettings.MonitorConfigurations.FirstOrDefault(mc => mc.Name == monitorName && IsNewConfig(mc.SourceIp))?.SourceIp;
         if (configValue != null)
         {
            _logger.LogInformation($"Configuration file (monitorSettings.json) overrides source IP for {monitorName} to {configValue}.");
         }

         return configValue;
      }

      private async Task WaitForMonitorConfiguration(CancellationToken stoppingToken)
      {
         if (_monitorsLoaded)
            return;
         
         while (!_statusMonitorConfiguration.Monitors.Any() && !stoppingToken.IsCancellationRequested)
         {
            await Task.Delay(10000, stoppingToken);
         }

         _monitorsLoaded = true;
      }
   }
}