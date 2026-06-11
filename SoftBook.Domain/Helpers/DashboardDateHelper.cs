using System;

namespace SoftBook.Domain.Helpers
{
    public static class DashboardDateHelper
    {
        private static readonly TimeZoneInfo BusinessTimeZone = ResolveBusinessTimeZone();

        public static DateTime GetBusinessToday()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BusinessTimeZone).Date;
        }

        private static TimeZoneInfo ResolveBusinessTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");
            }
            catch (InvalidTimeZoneException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");
            }
        }
    }
}
