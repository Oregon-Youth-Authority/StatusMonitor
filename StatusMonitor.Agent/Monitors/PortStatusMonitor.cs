using System;
using System.Threading.Tasks;
using State.Or.Oya.Jjis.StatusMonitor.Util;
using State.Or.Oya.StatusMonitor.Client.Generated;

namespace State.Or.Oya.Jjis.StatusMonitor.Monitors
{
   public class PortStatusMonitor : StatusMonitorBase<PortStatusMonitorConfig>
   {
      private readonly INetworkUtil _networkUtil;
      private readonly int _slowTime;



      public PortStatusMonitor(INetworkUtil networkUtil, MonitorConfiguration configuration) : base(configuration)
      {
         _networkUtil = networkUtil;
         _slowTime = Configuration.SlowTime ?? Convert.ToInt32(Configuration.Timeout * .4);
      }

      protected override async Task<MonitorStatus> GetCurrentStatus()
      {
         try
         {
            var time = await _networkUtil.GetTimeToConnect(Configuration.Host, Configuration.Port);
            return time > _slowTime ? MonitorStatus.Slow : MonitorStatus.Up;
         }
         catch (TimeoutException)
         {
            return MonitorStatus.Timeout;
         }
         catch
         {
            return MonitorStatus.Down;
         }
      }
   }
}
