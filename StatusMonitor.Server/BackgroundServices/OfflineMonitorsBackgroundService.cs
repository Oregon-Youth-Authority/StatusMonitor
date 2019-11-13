using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationStatusMonitor.Abstractions;
using ApplicationStatusMonitor.Controllers;
using Microsoft.Extensions.Logging;

namespace ApplicationStatusMonitor.BackgroundServices
{
   public class OfflineMonitorsBackgroundService : BackgroundService
   {
      private readonly IStatusRepository<StatusMonitorReply> _statusRepository;
      private readonly ILogger<OfflineMonitorsBackgroundService> _logger;

      public OfflineMonitorsBackgroundService(IStatusRepository<StatusMonitorReply> statusRepository, ILogger<OfflineMonitorsBackgroundService> logger)
      {
         _statusRepository = statusRepository;
         _logger = logger;
      }

      protected override async Task ExecuteAsync(CancellationToken stoppingToken)
      {
         while (!stoppingToken.IsCancellationRequested)
         {
            try
            {
               var datedStatusRecords = _statusRepository
                  .GetLatestStatusRecordForEachLocation()
                  .Where(x => x.LastStatusUpdateTime < DateTime.Now.AddHours(-2))
                  .ToList();

               foreach (var datedStatusRecord in datedStatusRecords)
               {
                  _statusRepository.AddStatusRecord(new StatusMonitorReply
                  {
                     Status = "Offline",
                     MonitorName = datedStatusRecord.MonitorName,
                     DisplayName = datedStatusRecord.DisplayName,
                     LocationId = datedStatusRecord.LocationId,
                     LastStatusUpdateTime = DateTime.Now,
                     StatusStartTime = DateTime.Now
                  });
               }

            }
            catch(Exception ex)
            {
               _logger.LogError(ex.Message);
            }
            finally
            {
               await Task.Delay(1000 * 60 * 15, stoppingToken);  // 15 minutes
            }
         }
      }
   }
}