using System.Collections.Generic;

namespace ApplicationStatusMonitor.Controllers
{
  public interface IMonitorConfigurationRepository<T>
  {
    T GetMonitorConfiguration(string monitorName);
    IEnumerable<T> GetAll();
    IEnumerable<T> GetByType(string type);
  }
}