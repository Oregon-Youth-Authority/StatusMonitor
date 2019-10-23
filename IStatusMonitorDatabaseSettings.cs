namespace ApplicationStatusMonitor
{
   public interface IStatusMonitorDatabaseSettings
   {
      string ConnectionString { get; set; }
      string StatusMonitorRepliesCollection { get; set; }
      string DatabaseName { get; set; }
   }
}