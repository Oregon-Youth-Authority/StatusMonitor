using System;

namespace ApplicationStatusMonitor.Extensions
{
   public static class DateTimeExtension
   {
      public static DateTime ToPacificStandardTime(this DateTime dateTime)
      {
         return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles"));
      }
   }
}