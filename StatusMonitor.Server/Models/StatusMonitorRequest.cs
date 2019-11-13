namespace ApplicationStatusMonitor.Controllers
{
   public class StatusMonitorRequest
   {
      public string DisplayName {get;set;}
      public string Status { get; set; }
      public string MonitorName { get; set; }
      public string LocationId { get; set; }
   }
}