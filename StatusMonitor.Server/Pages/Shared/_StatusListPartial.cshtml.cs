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
    }
}