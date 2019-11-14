using System;
using System.Threading.Tasks;

namespace StatusMonitor.ApiKey.Providers
{
   public class ApiKeyEvents
   {
      public Func<AuthenticationFailedContext, Task> OnAuthenticationFailed { get; set; } = context => Task.CompletedTask;

      public Func<MessageReceivedContext, Task> OnMessageReceived { get; set; } = context => Task.CompletedTask;

      public Func<ApiKeyValidatedContext, Task> OnApiKeyValidated { get; set; } = context => Task.CompletedTask;

      public Func<ApiKeyChallengeContext, Task> OnChallenge { get; set; } = context => Task.CompletedTask;

      public virtual Task AuthenticationFailed(AuthenticationFailedContext context) => OnAuthenticationFailed(context);

      public virtual Task MessageReceived(MessageReceivedContext context) => OnMessageReceived(context);

      public virtual Task ApiKeyValidated(ApiKeyValidatedContext context) => OnApiKeyValidated(context);

      public virtual Task Challenge(ApiKeyChallengeContext context) => OnChallenge(context);
   }
}