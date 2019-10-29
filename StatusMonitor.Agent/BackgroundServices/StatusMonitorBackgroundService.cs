using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using State.Or.Oya.Jjis.StatusMonitor.Configuration;
using State.Or.Oya.StatusMonitor.Client.Generated;

namespace State.Or.Oya.Jjis.StatusMonitor.BackgroundServices
{
   public class StatusMonitorBackgroundService : BackgroundService
   {
      private readonly StatusMonitorConfiguration _statusMonitorConfiguration;
      private readonly StatusMonitorClientFactory _statusMonitorClientFactory;
      private readonly ILogger<StatusMonitorBackgroundService> _logger;
      private readonly int _statusCheckInterval;
      private readonly int _statusUpdateInterval;
      private bool _monitorsLoaded = false;

      public StatusMonitorBackgroundService(ApplicationConfiguration applicationConfiguration, StatusMonitorConfiguration statusMonitorConfiguration, StatusMonitorClientFactory statusMonitorClientFactory, ILogger<StatusMonitorBackgroundService> logger)
      {
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
            foreach (var monitor in _statusMonitorConfiguration.Monitors)
            {
               if (await monitor.HasStatusChanged())
               {
                  var client = _statusMonitorClientFactory.Create();
                  await client.UpdateStatusAsync(new StatusMonitorRequest
                  {
                     MonitorName = monitor.Name,
                     Status = monitor.Status.ToString(),
                     DisplayName = Environment.MachineName
                  }, stoppingToken);
                  _logger.LogInformation($"{DateTime.Now} {monitor.Name} has changed from {monitor.PreviousStatus} to {monitor.Status}");
                  continue;
               }

               if (monitor.LastStatusChange < DateTime.Now.AddMilliseconds(_statusUpdateInterval * -1))
               {
                  _logger.LogInformation($"{DateTime.Now} {monitor.Name} is {monitor.Status}");
               }
            }

            await Task.Delay(_statusCheckInterval, stoppingToken);
         }
      }

      private async Task WaitForMonitorConfiguration(CancellationToken stoppingToken)
      {
         if (_monitorsLoaded)
            return;
         
         while (!_statusMonitorConfiguration.Monitors.Any() && !stoppingToken.IsCancellationRequested)
         {
            await Task.Delay(2000, stoppingToken);
         }

         _monitorsLoaded = true;
      }
   }
}