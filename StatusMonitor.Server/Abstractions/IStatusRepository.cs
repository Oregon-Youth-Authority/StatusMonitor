using System;
using System.Collections.Generic;

namespace ApplicationStatusMonitor.Controllers
{
   public interface IStatusRepository<T>
   {
      T AddStatusRecord(T record);
      T GetLatestStatusRecord(string location, string monitorName, string displayName);
      T Update(string location, string monitorName, string displayName, DateTime lastStatusUpdateTime);
      IEnumerable<T> GetLatestStatusRecordForEachLocation();
      IEnumerable<T> GetCurrentlyDown();
      
   }
}