namespace ApplicationStatusMonitor
{
   public class StatusMonitorDatabaseSettings : IStatusMonitorDatabaseSettings
   {
      public string ConnectionString { get; set; }
      public string StatusMonitorRepliesCollection { get; set; }
      public string DatabaseName { get; set; }
   }
}