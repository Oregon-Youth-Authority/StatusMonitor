using System;

namespace ApplicationStatusMonitor.Utility
{
   public static class DurationUtility
   {
      public static string GetDuration(DateTime startDateTime, DateTime endDateTime)
      {
         var duration = endDateTime - startDateTime;
         if (duration.TotalDays > 900)
            return $"{Math.Round(duration.TotalDays / 365.25)} years";
         if (duration.TotalDays > 35)
            return $"{Math.Round(duration.TotalDays / 7)} weeks";
         if (duration.TotalHours >= 48)
            return $"{Math.Round(duration.TotalDays)} days";
         if (duration.TotalMinutes >= 120)
            return $"{Math.Round(duration.TotalHours)} hours";
         
         var minutes = (int) Math.Round(duration.TotalMinutes);
         return minutes != 1
            ? $"{minutes} minutes"
            : $"{minutes} minute";
      }
   }
}