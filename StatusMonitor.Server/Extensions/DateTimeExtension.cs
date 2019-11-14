using System;

namespace ApplicationStatusMonitor.Extensions
{
   public static class DateTimeExtension
   {
      public static DateTime ToPacificStandardTime(this DateTime dateTime)
      {
         return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles"));
      }

      public static string ToPacificStandardTimeString(this DateTime dateTime)
      {
         return dateTime.ToPacificStandardTime().ToString("M/d/yyyy H:mm");
      }
   }
}