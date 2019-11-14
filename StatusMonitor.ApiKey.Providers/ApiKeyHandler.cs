using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace StatusMonitor.ApiKey.Providers
{
   public class ApiKeyHandler : AuthenticationHandler<ApiKeyOptions, ApiKeyEvents>
  {
    public ApiKeyHandler(
      IOptionsMonitor<ApiKeyOptions> options,
      ILoggerFactory logger,
      UrlEncoder encoder,
      ISystemClock clock)
      : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
         Exception authException;
         try
         {
            var messageReceivedContext = new MessageReceivedContext(Context, Scheme, Options);
            await Events.MessageReceived(messageReceivedContext);
            var apiKey = messageReceivedContext.ApiKey;

            if (string.IsNullOrEmpty(apiKey))
            {
               var header = Request.Headers[Options.Header].ToString();

               if (string.IsNullOrEmpty(header))
               {
                  Logger.ApiKeyValidationFailed();
                  return AuthenticateResult.NoResult();
               }

               if (!header.StartsWith(Options.HeaderKey, StringComparison.OrdinalIgnoreCase))
                  return AuthenticateResult.NoResult();

               apiKey = header.Substring(Options.HeaderKey.Length).Trim();
            }

            var validateApiKeyContext = new ApiKeyValidatedContext(Context, Scheme, Options)
            {
               ApiKey = apiKey
            };

            await Events.ApiKeyValidated(validateApiKeyContext);

            var authResult = validateApiKeyContext.Result ?? AuthenticateResult.NoResult();
            if (authResult.Succeeded)
            {
               Logger.ApiKeyValidationSucceeded();
               authResult.Principal.AddIdentity(new ClaimsIdentity(Scheme.Name));
            }
            else
               Logger.ApiKeyValidationFailed();

            return authResult;
         }
         catch (Exception ex)
         {
            authException = ex;
         }

         if (authException != null)
         {
           Logger.ErrorProcessingMessage(authException);
           var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
           {
             Exception = authException
           };

           await Events.AuthenticationFailed(authenticationFailedContext);

           if (authenticationFailedContext.Result != null)
             return authenticationFailedContext.Result;
         }
         
         return null;
    }

      protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
       {
         var authResult = await HandleAuthenticateOnceSafeAsync();
         if (authResult.Succeeded)
           return;

         var challengeContext = new ApiKeyChallengeContext(Context, Scheme, Options, properties)
         {
           AuthenticateFailure = authResult.Failure
         };

         await Events.Challenge(challengeContext);
         
         if (challengeContext.Handled)
           return;
         
         Response.StatusCode = (int) challengeContext.StatusCode;
       }
  }
}