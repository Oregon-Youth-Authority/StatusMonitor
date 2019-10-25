using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using State.Or.Oya.Jjis.StatusMonitor.Monitors;
using State.Or.Oya.Jjis.StatusMonitor.Util;
using State.Or.Oya.StatusMonitor.Client.Generated;

namespace State.Or.Oya.Jjis.StatusMonitor.BackgroundServices
{
   public class ConfigurationUpdaterBackgroundService : BackgroundService
   {
      private readonly StatusMonitorConfiguration _statusMonitorConfiguration;
      private readonly StatusMonitorClient _statusMonitorApi;
      private readonly INetworkUtil _networkUtil;
      private readonly ILogger<ConfigurationUpdaterBackgroundService> _logger;

      public ConfigurationUpdaterBackgroundService(StatusMonitorConfiguration statusMonitorConfiguration, StatusMonitorClient statusMonitorApi, INetworkUtil networkUtil, ILogger<ConfigurationUpdaterBackgroundService> logger)
      {
         _statusMonitorConfiguration = statusMonitorConfiguration;
         _statusMonitorApi = statusMonitorApi;
         _networkUtil = networkUtil;
         _logger = logger;
      }

      protected override async Task ExecuteAsync(CancellationToken stoppingToken)
      {
         while (!stoppingToken.IsCancellationRequested)
         {
            try
            {
                  var monitorConfigs = (await _statusMonitorApi
                     .GetConfigurationsAsync(stoppingToken))
                     .Where(m => m.Active)
                     .ToList();

                  foreach (var monitorConfiguration in monitorConfigs)
                  {
                     var existingMonitor = _statusMonitorConfiguration.Monitors.FirstOrDefault(m => m.Name == monitorConfiguration.MonitorName);
                     if (existingMonitor != null)
                        UpdateMonitor(existingMonitor, monitorConfiguration);
                     else
                     {
                        AddMonitor(monitorConfiguration);
                     }
                  }

                  _statusMonitorConfiguration.Monitors
                     .Where(m => monitorConfigs.All(config => config.MonitorName != m.Name))
                     .ToList()
                     .ForEach(m => _statusMonitorConfiguration.Monitors.Remove(m));
            }
            catch (Exception ex)
            {
               _logger.LogError($"Unable to refresh configuration. {ex.Message}");
            }
#if DEBUG
            await Task.Delay(10000, stoppingToken);
#else
            await Task.Delay(600000, stoppingToken);
#endif
         }
      }

      private void AddMonitor(MonitorConfiguration monitorConfiguration)
      {
         _statusMonitorConfiguration.Monitors.Add(StatusMonitorFactory.Create(monitorConfiguration, _networkUtil));
      }

      private void UpdateMonitor(IStatusMonitor existingMonitor, MonitorConfiguration monitorConfiguration)
      {
         var updatedMonitor = StatusMonitorFactory.Create(monitorConfiguration, _networkUtil);
         updatedMonitor.CopyStatusFrom(existingMonitor);
         _statusMonitorConfiguration.Monitors.Remove(existingMonitor);
         _statusMonitorConfiguration.Monitors.Add(updatedMonitor);
      }
   }
}