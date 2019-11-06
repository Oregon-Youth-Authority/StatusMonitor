using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationStatusMonitor.Abstractions
{
   public interface IStatusRepository<T>
   {
      T AddStatusRecord(T record);
      T GetLatestStatusRecord(string location, string monitorName, string displayName);
      T Update(string location, string monitorName, string displayName, DateTime lastStatusUpdateTime);
      IEnumerable<T> GetLatestStatusRecordForEachLocation();
      IEnumerable<T> GetCurrentlyDown();

      Task<IEnumerable<T>> GetStatusMonitorRepliesOlderThan(DateTime dateTime);
      Task DeleteStatusRecords(IEnumerable<T> statusRecordsToDelete);


   }
}