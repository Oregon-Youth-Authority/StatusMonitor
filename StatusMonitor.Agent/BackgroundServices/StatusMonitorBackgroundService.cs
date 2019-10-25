using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using State.Or.Oya.StatusMonitor.Client.Generated;

namespace State.Or.Oya.Jjis.StatusMonitor.BackgroundServices
{
   public class StatusMonitorBackgroundService : BackgroundService
   {
      private readonly StatusMonitorConfiguration _configuration;
      private readonly StatusMonitorClientFactory _statusMonitorClientFactory;
      private readonly ILogger<StatusMonitorBackgroundService> _logger;

      public StatusMonitorBackgroundService(StatusMonitorConfiguration configuration, StatusMonitorClientFactory statusMonitorClientFactory, ILogger<StatusMonitorBackgroundService> logger)
      {
         _configuration = configuration;
         _statusMonitorClientFactory = statusMonitorClientFactory;
         _logger = logger;
      }

      protected override async Task ExecuteAsync(CancellationToken stoppingToken)
      {
         while (!stoppingToken.IsCancellationRequested)
         {
            foreach (var monitor in _configuration.Monitors)
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

               if (monitor.LastStatusChange < DateTime.Now.AddMinutes(-60))
               {
                  _logger.LogInformation($"{DateTime.Now} {monitor.Name} is {monitor.Status}");
               }
            }

            await Task.Delay(3000, stoppingToken);
         }
      }
   }
}