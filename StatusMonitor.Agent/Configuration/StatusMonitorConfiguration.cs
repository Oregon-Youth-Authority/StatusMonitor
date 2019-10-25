using System.Collections.Generic;
using State.Or.Oya.Jjis.StatusMonitor.Monitors;

namespace State.Or.Oya.Jjis.StatusMonitor.Configuration
{
   public class StatusMonitorConfiguration
   {
      public ICollection<IStatusMonitor> Monitors { get; internal set; } = new List<IStatusMonitor>();
   }
}