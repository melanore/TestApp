using System;

namespace TestApp.Core.Util
{
    public class DateTimeMapper
    {
        public static DateTime Normalize(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}