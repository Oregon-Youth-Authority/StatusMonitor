namespace State.Or.Oya.Jjis.StatusMonitor.Monitors
{
   public interface IStatusMonitor
   {
      string Name { get; }
      string PreviousStatus { get; }
      string GetStatus();
   } 
}
