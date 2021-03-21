using System;

namespace Infrastructure.Shared.Utils
{
    static public class DateTimeExtensions
    {
        static public DateTime Trim(this DateTime dateTime, long ticks)
            => new DateTime(dateTime.Ticks - dateTime.Ticks % ticks, dateTime.Kind);
        
        static public DateTime TrimToSeconds(this DateTime dateTime)
            => dateTime.Trim(TimeSpan.TicksPerSecond);
    }
}
