using System.Collections.Generic;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   public interface IStatusMonitorApi
   {
      IEnumerable<MonitorConfiguration> GetMonitorConfigs();
   }

   public class FakeStatusMonitorApi : IStatusMonitorApi
   {
      public IEnumerable<MonitorConfiguration> GetMonitorConfigs()
      {
         return new List<MonitorConfiguration>
            {
               new MonitorConfiguration
               {
                  Type = "Port",
                  Value = "{\"host\":\"localhost\",\"port\":44300}",
                  MonitorName = "JJIS Web",
                  Enabled = true
               }
            };
      }
   }

   public class MonitorConfiguration
   {
      public string Id { get; set; }

      public string MonitorName { get; set; }

      public string Value { get; set; }

      public string Type { get; set; }

      public bool Enabled { get; set; }

   }
}