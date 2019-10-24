namespace ApplicationStatusMonitor.Controllers
{
   public interface IMonitorConfigurationRepository<T>
   {
      T GetMonitorConfiguration(string monitorName);
   }
}