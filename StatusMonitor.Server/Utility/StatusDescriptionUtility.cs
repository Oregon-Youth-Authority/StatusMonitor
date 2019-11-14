namespace ApplicationStatusMonitor.Utility
{
   public static class StatusDescriptionUtility
   {
      public static string GetDescription(string status)
      {
         return status switch
         {
            "Up" => "This status indicates that the agent is running and able to connect.",
            "Down" => "This status indicates that the agent is running, but unable to connect.",
            "Slow" => "This status indicates that the agent is running and able to connect, but the operation took longer than expected.",
            "Timeout" => "This status indicates that the agent is running, but the connection attempt timed out.",
            "Offline" => "This status indicates that the agent is either not running or unable to reach the API on on this site.",
            _ => null
         };
      }
   }
}