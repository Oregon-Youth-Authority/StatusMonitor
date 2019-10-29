using System;
using State.Or.Oya.StatusMonitor.Client.Generated;
using System.Net;
using System.Threading.Tasks;

namespace State.Or.Oya.Jjis.StatusMonitor.Monitors
{
   public class HttpStatusMonitor : StatusMonitorBase<HttpStatusMonitorConfig>
   {
      public HttpStatusMonitor(MonitorConfiguration configuration) : base(configuration)
      {
      }

      protected override async Task<MonitorStatus> GetCurrentStatus()
      {
         var request = (HttpWebRequest)WebRequest.Create(Configuration.Url);
         try
         {
            request.Timeout = Configuration.Timeout;
            using var response = await request.GetResponseAsync();
            return MonitorStatus.Up;
         }
         catch (WebException ex)
         {
            return ex.Status switch
            {
               WebExceptionStatus.Timeout => MonitorStatus.Timeout,
               WebExceptionStatus.NameResolutionFailure => MonitorStatus.NameResolutionFailure,
               _ => MonitorStatus.Down
            };
         }
         catch
         {
            return MonitorStatus.Down;
         }
      }
   }
}