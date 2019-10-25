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

      public int ConfigurationRefreshInterval { get; set; } = 600000;
      public int StatusCheckInterval { get; set; } = 360000;
      public int StatusUpdateInterval { get; set; } = 3600000;

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
