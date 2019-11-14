using Microsoft.AspNetCore.Authentication;

namespace StatusMonitor.ApiKey.Providers
{
   public class ApiKeyOptions : AuthenticationSchemeOptions
   {
      public string Header { get; set; } = "Authorization";

      public string HeaderKey { get; set; } = "ApiKey";
      
      public ApiKeyOptions()
      {
         Events = new ApiKeyEvents();
      }
   }
}