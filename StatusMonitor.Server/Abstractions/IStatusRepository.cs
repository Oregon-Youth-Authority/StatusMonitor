namespace ApplicationStatusMonitor.Controllers
{
   public interface IStatusRepository<T>
   {
      T AddStatusRecord(T record);
      T GetLatestStatusRecord(string location, string monitorName);
   }
}