using System;
using System.Threading.Tasks;
using State.Or.Oya.Jjis.StatusMonitor.Util;
using State.Or.Oya.StatusMonitor.Client.Generated;

namespace State.Or.Oya.Jjis.StatusMonitor.Monitors
{
   public class PortStatusMonitorBase : StatusMonitorBase<PortStatusMonitorConfig>
   {
      private readonly INetworkUtil _networkUtil;
      private readonly int _slowTime;



      public PortStatusMonitorBase(INetworkUtil networkUtil, MonitorConfiguration configuration) : base(configuration)
      {
         _networkUtil = networkUtil;
         _slowTime = Configuration.SlowTime ?? Convert.ToInt32(Configuration.Timeout * .4);
      }

      public override async Task<bool> HasStatusChanged()
      {
         PreviousStatus = Status;
         Status = await GetCurrentStatus();
         if (Status == PreviousStatus)
            return false;
         LastStatusChange = DateTime.Now;
         return true;
      }

      protected virtual async Task<string> GetCurrentStatus()
      {
         try
         {
            var time = await _networkUtil.GetTimeToConnect(Configuration.Host, Configuration.Port);
            return time > _slowTime ? "Slow" : "Up";
         }
         catch (TimeoutException)
         {
            return "Timeout";
         }
         catch
         {
            return "Down";
         }
      }
   }
}
