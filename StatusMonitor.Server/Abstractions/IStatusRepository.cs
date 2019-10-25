using System;

namespace ApplicationStatusMonitor.Controllers
{
   public interface IStatusRepository<T>
   {
      T AddStatusRecord(T record);
      T GetLatestStatusRecord(string location, string monitorName);
      T Update(string location, string monitorName, DateTime lastStatusUpdateTime);
   }
}