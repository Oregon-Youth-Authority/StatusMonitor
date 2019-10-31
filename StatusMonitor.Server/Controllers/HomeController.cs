using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationStatusMonitor.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace StatusMonitor.Server.Controllers
{
    public class HomeController : Controller
    {
       private IStatusRepository<StatusMonitorReply> _replies;

       public HomeController(IStatusRepository<StatusMonitorReply> statusReplies)
       {
          _replies = statusReplies;
       }
        public IActionResult Index()
        {
            var latestStatusReplies = _replies.GetLatestStatusRecordForEachLocation();
            return View(latestStatusReplies);
        }
    }
}