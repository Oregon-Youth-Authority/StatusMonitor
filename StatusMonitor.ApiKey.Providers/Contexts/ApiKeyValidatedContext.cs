using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace StatusMonitor.ApiKey.Providers
{
   public class ApiKeyValidatedContext : ResultContext<ApiKeyOptions>
     {
        public ApiKeyValidatedContext(
           HttpContext context,
           AuthenticationScheme scheme,
           ApiKeyOptions options)
           : base(context, scheme, options)
        {
           Principal = new ClaimsPrincipal();
        }

        public string ApiKey { get; internal set; }
     }
}