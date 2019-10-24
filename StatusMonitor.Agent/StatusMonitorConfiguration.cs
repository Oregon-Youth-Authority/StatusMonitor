using System.Collections.Generic;
using System.Threading.Tasks;
using State.Or.Oya.Jjis.StatusMonitor.Monitors;

namespace State.Or.Oya.Jjis.StatusMonitor
{
   public class StatusMonitorConfiguration
   {
      public ICollection<IStatusMonitor> Monitors { get; internal set; } = new List<IStatusMonitor>();
   }
}