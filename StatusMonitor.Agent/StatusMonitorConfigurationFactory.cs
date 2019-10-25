using State.Or.Oya.Jjis.StatusMonitor.Util;
using System.Linq;
using System.Threading.Tasks;
using State.Or.Oya.StatusMonitor.Client.Generated;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   public static class StatusMonitorConfigurationFactory
   {
      public static async Task<StatusMonitorConfiguration> Create(StatusMonitorClient statusMonitorApi, INetworkUtil networkUtil)
      {
         try
         {
            var monitorConfigs = await statusMonitorApi.GetConfigurationsAsync();

            return new StatusMonitorConfiguration
            {
               Monitors = monitorConfigs
                  .Where(mc => mc.Active)
                  .Select(mc => StatusMonitorFactory.Create(mc, networkUtil))
                  .Where(monitor => monitor != null)
                  .ToList()
            };
         }
         catch
         {
            return new StatusMonitorConfiguration();
         }
      }
   }
}