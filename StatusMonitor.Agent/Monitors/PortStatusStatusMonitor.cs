namespace State.Or.Oya.Jjis.StatusMonitor.Monitors
{
   public class PortStatusStatusMonitor : IStatusMonitor
   {
      private readonly string _host;
      private readonly int _port;

      private string _status;
      private string _previousStatus;

      public virtual string Name { get; }

      public virtual string PreviousStatus => _previousStatus;

      public PortStatusStatusMonitor(string name, string host, int port)
      {
         Name = name;
         _host = host;
         _port = port;
      }

      public virtual string GetStatus()
      {
         _previousStatus = _status;
         _status = GetCurrentStatus();
         return _status;
      }

      protected virtual string GetCurrentStatus()
      {
         return "Up";
      }
   }
}
