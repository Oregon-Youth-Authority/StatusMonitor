using System.Collections.Generic;
using ApplicationStatusMonitor.Abstractions;
using ApplicationStatusMonitor.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ApplicationStatusMonitor.Pages
{
   [Authorize]
   public class IndexModel : PageModel
   {
      private readonly ILogger<IndexModel> _logger;
      private readonly IStatusRepository<StatusMonitorReply> _statusReplies;

      public IndexModel(IStatusRepository<StatusMonitorReply> statusReplies, ILogger<IndexModel> logger)
      {
         _logger = logger;
         _statusReplies = statusReplies;
      }

      public IEnumerable<StatusMonitorReply> StatusReplies { get; private set; }

      public void OnGet()
      {
         StatusReplies = _statusReplies.GetLatestStatusRecordForEachLocation();
      }
   }
}
