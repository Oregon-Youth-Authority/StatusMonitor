using System.Collections.Generic;
using ApplicationStatusMonitor.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApplicationStatusMonitor.Pages.Shared
{
    public class StatusListPartialModel : PageModel
    {
       public IEnumerable<StatusMonitorReply> StatusMonitorReplies { get; set; }
       public bool ShowConfirmed { get; set; } = true;
       public bool ShowDuration { get; set; } = true;
       public string ConfirmedCssClass => GetCssClassFromBool(ShowConfirmed);
       public string DurationCssClass => GetCssClassFromBool(ShowDuration);

       public string GetMaskedLocationId(string locationId)
       {
          if (locationId == null)
             return null;

          var sections = locationId.Split(new[] {'.'});
          return sections.Length < 4 
             ? "**.***.***.**" 
             : $"**.{sections[1]}.{sections[2]}.**";
       }

       private string GetCssClassFromBool(bool isVisible)
       {
          return isVisible ? string.Empty : "hidden";
       }

    }
}