using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace State.Or.Oya.Jjis.StatusMonitor.Monitors
{
   public abstract class StatusMonitor<TConfig> : IStatusMonitor
   {
      private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
      {
         PropertyNameCaseInsensitive = true
      };

      protected StatusMonitor(MonitorConfiguration configuration)
      {
         Configuration = JsonSerializer.Deserialize<TConfig>(configuration.Value, _jsonSerializerOptions);
         Name = configuration.MonitorName;
      }

      public string Name { get; }
      public virtual string PreviousStatus { get; protected set; }
      public DateTime LastStatusChange { get; protected set; } = DateTime.Today;

      public abstract Task<bool> HasStatusChanged();

      public virtual string Status { get; protected set; } = "Offline";

      public void CopyStatusFrom(IStatusMonitor copyFrom)
      {
         LastStatusChange = copyFrom.LastStatusChange;
         PreviousStatus = copyFrom.PreviousStatus;
         Status = copyFrom.Status;
      }

      protected TConfig Configuration { get; set; }
   }
}