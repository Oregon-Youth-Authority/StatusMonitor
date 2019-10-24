using System;

namespace State.Or.Oya.Jjis.StatusMonitor.Monitors
{
   public interface IStatusMonitor
   {
      string Name { get; }
      string PreviousStatus { get; }
      string Status { get; }

      DateTime LastStatusChange { get; }

      bool HasStatusChanged();
   } 
}
