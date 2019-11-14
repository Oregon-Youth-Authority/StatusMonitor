using System;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace StatusMonitor.ApiKey.Providers
{
   public class ApiKeyChallengeContext : PropertiesContext<ApiKeyOptions>
   {
      public ApiKeyChallengeContext(
         HttpContext context,
         AuthenticationScheme scheme,
         ApiKeyOptions options,
         AuthenticationProperties properties)
         : base(context, scheme, options, properties)
      {
      }

      public string ApiKey { get; set; }

      public Exception AuthenticateFailure { get; set; }

      public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Unauthorized;

      public bool Handled { get; private set; }

      public void HandleResponse()
      {
         Handled = true;
      }
   }
}