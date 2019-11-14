using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace StatusMonitor.ApiKey.Providers
{
   public class AuthenticationFailedContext : ResultContext<ApiKeyOptions>
   {
      public AuthenticationFailedContext(
         HttpContext context,
         AuthenticationScheme scheme,
         ApiKeyOptions options)
         : base(context, scheme, options)
      {
      }

      public Exception Exception { get; set; }
   }
}