using System;
using Microsoft.Extensions.Options;

namespace StatusMonitor.ApiKey.Providers
{
   public class ApiKeyPostConfigureOptions : IPostConfigureOptions<ApiKeyOptions>
   {
      public void PostConfigure(string name, ApiKeyOptions options)
      {
         if (string.IsNullOrWhiteSpace(options.Header))
            throw new ArgumentException("Header must have a value.", "Header");
         if (options.HeaderKey == null)
            throw new ArgumentException("Header key must not be null.", "HeaderKey");
      }
   }
}