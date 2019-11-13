using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace State.Or.Oya.Jjis.StatusMonitor.Configuration
{
   public class ApplicationConfiguration
   {
      public ApplicationConfiguration(IConfiguration configuration, ILogger<ApplicationConfiguration> logger)
      {
         configuration.Bind(this);
      }

      public int ConfigurationRefreshInterval { get; set; } = 900000; // 15 minutes
      public int StatusCheckInterval { get; set; } = 300000;  // 5 minutes
      public int StatusUpdateInterval { get; set; } = 3600000; // 60 minutes
   }
}
