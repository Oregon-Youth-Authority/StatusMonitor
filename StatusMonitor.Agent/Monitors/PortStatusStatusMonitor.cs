using System;

namespace State.Or.Oya.Jjis.StatusMonitor.Monitors
{
   public class PortStatusStatusMonitor : IStatusMonitor
   {
      private readonly string _host;
      private readonly int _port;

      private string _status;
      private string _previousStatus;
      private DateTime _lastStatusChange = DateTime.Today;

      public PortStatusStatusMonitor(string name, string host, int port)
      {
         Name = name;
         _host = host;
         _port = port;
      }

      public virtual string Name { get; }

      public virtual string PreviousStatus => _previousStatus;
      public DateTime LastStatusChange => _lastStatusChange;

      public virtual string Status => _status;

      public virtual bool HasStatusChanged()
      {
         _previousStatus = _status;
         _status = GetCurrentStatus();
         if (_status == _previousStatus)
            return false;
         _lastStatusChange = DateTime.Now;
         return true;
      }

      protected virtual string GetCurrentStatus()
      {
         return "Up";
      }
   }
}
