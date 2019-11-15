using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationStatusMonitor.Abstractions;
using ApplicationStatusMonitor.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ApplicationStatusMonitor.Pages
{
   public class ByMonitorModel : PageModel
   {
      private readonly IStatusRepository<StatusMonitorReply> _statusReplies;
      private readonly ILogger<MonitorStatusModel> _logger;

      public ByMonitorModel(IStatusRepository<StatusMonitorReply> statusReplies, ILogger<MonitorStatusModel> logger)
      {
         _statusReplies = statusReplies;
         _logger = logger;
      }

      public IEnumerable<StatusMonitorReply> StatusReplies { get; private set; }

      [BindProperty(SupportsGet = true)]
      public string MonitorName { get; set; }

      public void OnGet()
      {
         StatusReplies = _statusReplies.GetLatestStatusRecordForEachLocation().Where(x => x.MonitorName == MonitorName);
      }

   }
}