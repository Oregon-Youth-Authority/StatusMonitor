using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace StatusMonitor.ApiKey.Providers
{
   public class MessageReceivedContext : BaseContext<ApiKeyOptions>
     {
        public MessageReceivedContext(
           HttpContext context,
           AuthenticationScheme scheme,
           ApiKeyOptions options)
           : base(context, scheme, options)
        {
        }

        public string ApiKey { get; set; }
     }
}