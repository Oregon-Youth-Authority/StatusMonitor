using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationStatusMonitor.Abstractions;
using ApplicationStatusMonitor.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ApplicationStatusMonitor.Pages
{
   public class MonitorStatusModel : PageModel
   {
      private readonly IStatusRepository<StatusMonitorReply> _statusReplies;
      private readonly ILogger<MonitorStatusModel> _logger;

      public MonitorStatusModel(IStatusRepository<StatusMonitorReply> statusReplies, ILogger<MonitorStatusModel> logger)
      {
         _statusReplies = statusReplies;
         _logger = logger;
      }

      public IEnumerable<StatusMonitorReply> StatusReplies { get; private set; }


      [BindProperty(SupportsGet = true)]
      public string LocationId { get; set; }

      [BindProperty(SupportsGet = true)]
      public string Name { get; set; }

      public async Task OnGet()
      {
         StatusReplies = await _statusReplies.GetStatusRepliesForLocation(LocationId, Name);
      }

      public string FormattedName()
      {
         return Name == null ? LocationId : $"{Name} - {LocationId}";
      }
   }
}