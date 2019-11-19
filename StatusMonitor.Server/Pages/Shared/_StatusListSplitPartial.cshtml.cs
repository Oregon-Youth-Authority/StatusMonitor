using System.Collections.Generic;
using System.Linq;
using ApplicationStatusMonitor.Controllers;

namespace ApplicationStatusMonitor.Pages.Shared
{
    public class StatusListSplitPartialModel : StatusListPartialModel
    {
       public IEnumerable<StatusMonitorReply> OnlineReplies => StatusMonitorReplies.Where(x => x.Status != "Offline");
       public IEnumerable<StatusMonitorReply> OfflineReplies => StatusMonitorReplies.Where(x => x.Status == "Offline");


      public void OnGet()
        {

        }
    }
}