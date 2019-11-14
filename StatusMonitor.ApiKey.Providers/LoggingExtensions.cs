using System;
using Microsoft.Extensions.Logging;

namespace StatusMonitor.ApiKey.Providers
{
   internal static class LoggingExtensions
   {
      private static readonly Action<ILogger, Exception> apiKeyValidationFailed = LoggerMessage.Define(LogLevel.Information, 1, "Failed to validate the API key.");
      private static readonly Action<ILogger, Exception> apiKeyValidationSucceeded = LoggerMessage.Define(LogLevel.Information, 2, "Successfully validated the API key.");
      private static readonly Action<ILogger, Exception> errorProcessingMessage = LoggerMessage.Define(LogLevel.Error, 3, "Exception occurred while processing message.");

      public static void ApiKeyValidationFailed(this ILogger logger) => apiKeyValidationFailed(logger, null);

      public static void ApiKeyValidationSucceeded(this ILogger logger) => apiKeyValidationSucceeded(logger, null);

      public static void ErrorProcessingMessage(this ILogger logger, Exception ex) => errorProcessingMessage(logger, ex);
   }
}