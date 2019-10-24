using State.Or.Oya.Jjis.StatusMonitor.Util;
using System.Linq;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   public static class StatusMonitorConfigurationFactory
   {
      public static StatusMonitorConfiguration Create(IStatusMonitorApi statusMonitorApi, INetworkUtil networkUtil)
      {
         try
         {
            var monitorConfigs = statusMonitorApi.GetMonitorConfigs();

            return new StatusMonitorConfiguration
            {
               Monitors = monitorConfigs
                  .Where(mc => mc.Enabled)
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