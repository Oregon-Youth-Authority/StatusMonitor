using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace State.Or.Oya.Jjis.StatusMonitor.Util
{
   public interface INetworkUtil
   {
      Task<float> GetTimeToConnect(string host, int port = 443, int timeout = 5000);
   }

   public class NetworkUtil : INetworkUtil
   {
      private const int MaxTimeout = 30000;

      public async Task<float> GetTimeToConnect(string host, int port = 443, int timeout = 5000)
      {
         timeout = Math.Min(timeout, MaxTimeout);

         using var client = new TcpClient();
         try
         {
            var connectTask = client.ConnectAsync(host, port);
            var timeoutTask = Task.Delay(timeout);

            var sw = Stopwatch.StartNew();
            if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
            {
               throw new TimeoutException();
            }

            sw.Stop();
            if (connectTask.Exception?.InnerException != null)
            {
               throw connectTask.Exception.InnerException;
            }

            var time = (float)Math.Round((float)sw.ElapsedTicks / 10000, 2);

            return time;
         }
         finally
         {
            client.Close();
         }
      }
   }
}