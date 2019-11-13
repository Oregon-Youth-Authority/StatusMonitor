using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace State.Or.Oya.Jjis.StatusMonitor.Configuration
{
   public class MonitorSettings
   {
      public MonitorSettings(IConfiguration configuration)
      {
         configuration.Bind(this);
      }
   
      public IEnumerable<MonitorSettingsConfiguration> MonitorConfigurations { get; set; }
   }

   public class MonitorSettingsConfiguration
   {
      public string Name { get; set; }
      public string SourceIp { get; set; }
      public string Computer { get; set; }
   }
}
