namespace ApplicationStatusMonitor.Abstractions
{
   public interface IStatusMonitorDatabaseSettings
   {
      string ConnectionString { get; set; }
      string StatusMonitorRepliesCollection { get; set; }
      string MonitorConfigsCollection { get; set; }
      string DatabaseName { get; set; }
   }
}