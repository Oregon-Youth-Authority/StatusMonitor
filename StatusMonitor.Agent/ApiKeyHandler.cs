using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   internal class ApiKeyHandler : DelegatingHandler
   {
      private ApiKeySettings _apiKeyOptions;

      public ApiKeyHandler(IOptions<ApiKeySettings> options)
      {
         _apiKeyOptions = options.Value;
      }
      #region Overrides of DelegatingHandler

      protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         request.Headers.Remove("X-API-KEY");
         request.Headers.Add("X-API-KEY", _apiKeyOptions.ApiKey);
         return base.SendAsync(request, cancellationToken);
      }

      #endregion
   }
}