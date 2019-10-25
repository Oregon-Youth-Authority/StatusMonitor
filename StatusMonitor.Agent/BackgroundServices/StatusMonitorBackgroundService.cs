using System;
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
                     Status = monitor.Status,
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
   }
}