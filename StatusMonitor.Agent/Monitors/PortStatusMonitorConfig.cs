namespace State.Or.Oya.Jjis.StatusMonitor.Monitors
{
   public class PortStatusMonitorConfig
   {
      public string Host { get; set; }
      public int Port { get; set; } = 443;
      public int Timeout { get; set; } = 5000;
      public int? SlowTime { get; set; }
   }
}
