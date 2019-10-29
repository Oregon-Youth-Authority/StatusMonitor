using System;
using System.Collections.Generic;
using System.Text;

namespace State.Or.Oya.Jjis.StatusMonitor.Monitors
{
   public class HttpStatusMonitorConfig
   {
      public string Url { get; set; }
      public int Timeout { get; set; } = 5000;
   }
}
