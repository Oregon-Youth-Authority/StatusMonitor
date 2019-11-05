using ApplicationStatusMonitor.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ApplicationStatusMonitor.Pages
{
   public class UnhealthyModel : PageModel
   {
      private readonly ILogger<UnhealthyModel> _logger;
      private readonly IStatusRepository<StatusMonitorReply> _statusReplies;

      public UnhealthyModel(IStatusRepository<StatusMonitorReply> statusReplies, ILogger<UnhealthyModel> logger)
      {
         _logger = logger;
         _statusReplies = statusReplies;
      }

      public IEnumerable<StatusMonitorReply> StatusReplies { get; private set; }

      public void OnGet()
      {
         StatusReplies = _statusReplies.GetCurrentlyDown();
      }
   }
}