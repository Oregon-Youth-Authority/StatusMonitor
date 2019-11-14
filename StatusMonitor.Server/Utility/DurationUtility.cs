using System;

namespace ApplicationStatusMonitor.Utility
{
   public static class DurationUtility
   {
      public static string GetDuration(DateTime startDateTime, DateTime endDateTime)
      {
         var duration = endDateTime - startDateTime;
         if (duration.TotalDays > 2)
            return $"{Math.Round(duration.TotalDays)} days";
         return duration.TotalHours > 2 
            ? $"{Math.Round(duration.TotalHours)} hours" 
            : $"{Math.Round(duration.TotalMinutes)} minutes";
      }
   }
}