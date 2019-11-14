using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace StatusMonitor.ApiKey.Providers
{
   public static class AuthenticationBuilderExtensions
   {
      public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder) 
         => builder.AddApiKey("ApiKey", _ => { });

      public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, Action<ApiKeyOptions> configureOptions) 
         => builder.AddApiKey("ApiKey", configureOptions);

      public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, Action<ApiKeyOptions> configureOptions) 
         => builder.AddApiKey(authenticationScheme, "API Key", configureOptions);

      public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<ApiKeyOptions> configureOptions)
      {
         builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<ApiKeyOptions>, ApiKeyPostConfigureOptions>());
         builder.AddScheme<ApiKeyOptions, ApiKeyHandler>(authenticationScheme, displayName, configureOptions);
         return builder;
      }
   }
}