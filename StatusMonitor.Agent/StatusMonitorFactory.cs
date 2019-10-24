using State.Or.Oya.Jjis.StatusMonitor.Monitors;
using State.Or.Oya.Jjis.StatusMonitor.Util;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   public static class StatusMonitorFactory
   {
      public static IStatusMonitor Create(MonitorConfiguration configuration, INetworkUtil networkUtil)
      {
         switch (configuration.Type)
         {
            case "Port":
               return new PortStatusMonitor(networkUtil, configuration);

            default:
               return null;
         }
      }
   }
}