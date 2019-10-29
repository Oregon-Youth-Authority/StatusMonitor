using System;
using System.Text.Json;
using System.Threading.Tasks;
using State.Or.Oya.StatusMonitor.Client.Generated;

namespace State.Or.Oya.Jjis.StatusMonitor.Monitors
{
   public abstract class StatusMonitorBase<TConfig> : IStatusMonitor
   {
      private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
      {
         PropertyNameCaseInsensitive = true
      };

      protected StatusMonitorBase(MonitorConfiguration configuration)
      {
         Configuration = JsonSerializer.Deserialize<TConfig>(configuration.Value, _jsonSerializerOptions);
         Name = configuration.MonitorName;
      }

      public string Name { get; }
      public virtual MonitorStatus PreviousStatus { get; protected set; } = MonitorStatus.Offline;
      public DateTime LastStatusChange { get; protected set; } = DateTime.Today;

      public virtual MonitorStatus Status { get; protected set; } = MonitorStatus.Offline;

      public virtual async Task<bool> HasStatusChanged()
      {
         PreviousStatus = Status;
         Status = await GetCurrentStatus();
         if (Status == PreviousStatus)
            return false;
         LastStatusChange = DateTime.Now;
         return true;
      }
      
      public void CopyStatusFrom(IStatusMonitor copyFrom)
      {
         LastStatusChange = copyFrom.LastStatusChange;
         PreviousStatus = copyFrom.PreviousStatus;
         Status = copyFrom.Status;
      }

      protected abstract Task<MonitorStatus> GetCurrentStatus();

      protected TConfig Configuration { get; set; }
   }
}