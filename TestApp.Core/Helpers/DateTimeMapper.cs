using System;

namespace TestApp.Core.Helpers
{
    public class DateTimeMapper
    {
        public static DateTime Normalize(DateTime value)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}