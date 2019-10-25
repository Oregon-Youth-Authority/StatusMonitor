using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace State.Or.Oya.Jjis.StatusMonitor.Configuration
{
   public class ApplicationConfiguration
   {
      private readonly ILogger<ApplicationConfiguration> _logger;

      public ApplicationConfiguration(IConfiguration configuration, ILogger<ApplicationConfiguration> logger)
      {
         _logger = logger;
         ConfigurationRefreshInterval = TryGetValueFromConfig(configuration, nameof(ConfigurationRefreshInterval), ConfigurationRefreshInterval);
         StatusCheckInterval = TryGetValueFromConfig(configuration, nameof(StatusCheckInterval), StatusCheckInterval);
         StatusUpdateInterval = TryGetValueFromConfig(configuration, nameof(StatusUpdateInterval), StatusUpdateInterval);
      }

      public int ConfigurationRefreshInterval { get; set; } = 900000; // 15 minutes
      public int StatusCheckInterval { get; set; } = 300000;  // 5 minutes
      public int StatusUpdateInterval { get; set; } = 3600000; // 60 minutes

      private int TryGetValueFromConfig(IConfiguration config, string configName, int defaultValue)
      {
         try
         {
            return config.GetValue<int>(configName);
         }
         catch (Exception ex)
         {
            _logger.LogWarning($"Unable to read {configName} from configuration. {ex.Message}");
            return defaultValue;
         }
      }
   }
}
