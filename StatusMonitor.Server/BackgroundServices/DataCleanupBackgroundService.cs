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
   public class DataCleanupBackgroundService : BackgroundService
   {
      private readonly IStatusRepository<StatusMonitorReply> _statusRepository;
      private readonly ILogger<DataCleanupBackgroundService> _logger;

      public DataCleanupBackgroundService(IStatusRepository<StatusMonitorReply> statusRepository, ILogger<DataCleanupBackgroundService> logger)
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
               var currentStatusRecordIds = _statusRepository
                  .GetLatestStatusRecordForEachLocation()
                  .Select(sr => sr.Id)
                  .ToList();

               var oldRecords = await _statusRepository.GetStatusMonitorRepliesOlderThan(DateTime.Now.AddDays(-14));
               var recordsToDelete = oldRecords.Where(sr => currentStatusRecordIds.All(id => sr.Id != id)).ToList();
               await _statusRepository.DeleteStatusRecords(recordsToDelete);

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