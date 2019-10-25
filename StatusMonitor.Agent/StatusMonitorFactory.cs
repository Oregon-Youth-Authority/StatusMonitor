using System;
using System.Threading.Tasks;
using State.Or.Oya.Jjis.StatusMonitor.Monitors;
using State.Or.Oya.Jjis.StatusMonitor.Util;
using State.Or.Oya.StatusMonitor.Client.Generated;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   public static class StatusMonitorFactory
   {
      public static IStatusMonitor Create(MonitorConfiguration configuration, INetworkUtil networkUtil)
      {
         switch (configuration.Type)
         {
            case "Port":
               return new PortStatusMonitorBase(networkUtil, configuration);

            default:
               return new EmptyStatusMonitor();
         }
      }
   }

   public class EmptyStatusMonitor : IStatusMonitor
   {
      public string Name { get; } = "Empty";
      public string PreviousStatus => Name;
      public string Status => Name;
      public DateTime LastStatusChange { get; } = DateTime.MinValue;
      public Task<bool> HasStatusChanged()
      {
         return Task.FromResult(false);
      }

      public void CopyStatusFrom(IStatusMonitor copyFrom) { }
   }
}