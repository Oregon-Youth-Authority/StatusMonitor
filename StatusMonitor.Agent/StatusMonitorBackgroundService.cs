using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   public class StatusMonitorBackgroundService : BackgroundService
   {
      private readonly StatusMonitorConfiguration _configuration;
      private readonly ILogger<StatusMonitorBackgroundService> _logger;

      public StatusMonitorBackgroundService(StatusMonitorConfiguration configuration, ILogger<StatusMonitorBackgroundService> logger)
      {
         _configuration = configuration;
         _logger = logger;
      }

      protected override async Task ExecuteAsync(CancellationToken stoppingToken)
      {
         while (!stoppingToken.IsCancellationRequested)
         {
            foreach (var monitor in _configuration.Monitors)
            {
               var currentStatus = monitor.GetStatus();
               if (currentStatus != monitor.PreviousStatus)
               {
                  _logger.LogInformation($"{DateTime.Now} {monitor.Name} is {currentStatus}");
               }
            }

            await Task.Delay(3000, stoppingToken);
         }
      }
   }
}