using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationStatusMonitor.Abstractions;
using ApplicationStatusMonitor.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ApplicationStatusMonitor.Pages
{
   public class RecentUnhealthyModel : PageModel
   {
      private readonly ILogger<RecentUnhealthyModel> _logger;
      private readonly IStatusRepository<StatusMonitorReply> _statusReplies;

      public RecentUnhealthyModel(IStatusRepository<StatusMonitorReply> statusReplies, ILogger<RecentUnhealthyModel> logger)
      {
         _logger = logger;
         _statusReplies = statusReplies;
      }

      public IEnumerable<StatusMonitorReply> StatusReplies { get; private set; }

      public async Task OnGet()
      {
         StatusReplies = await _statusReplies.GetRecentlyUnhealthy(DateTime.Now.AddDays(-3));
      }
   }
}