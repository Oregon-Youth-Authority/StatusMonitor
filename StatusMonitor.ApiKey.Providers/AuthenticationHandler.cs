using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace StatusMonitor.ApiKey.Providers
{
   public abstract class AuthenticationHandler<TOptions, TEvents> : AuthenticationHandler<TOptions>
      where TOptions : AuthenticationSchemeOptions, new()
   {
      protected AuthenticationHandler(
         IOptionsMonitor<TOptions> options,
         ILoggerFactory logger,
         UrlEncoder encoder,
         ISystemClock clock)
         : base(options, logger, encoder, clock)
      {
      }

      protected TEvents Events
      {
         get => (TEvents) base.Events;
         set => Events = value;
      }
   }
}