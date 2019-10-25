using System;
using System.Collections.Generic;
using System.Text;

namespace State.Or.Oya.StatusMonitor.Client.Generated
{
   public partial class StatusMonitorClient
   {
      public string BaseUrl => _httpClient.BaseAddress.ToString();
   }
}
