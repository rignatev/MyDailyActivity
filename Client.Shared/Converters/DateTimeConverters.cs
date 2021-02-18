using System;

using Avalonia.Data.Converters;

namespace Client.Shared.Converters
{
    static public class DateTimeConverters
    {
        static public readonly IValueConverter UtcToLocal = new FuncTwoWayValueConverter<DateTime, DateTime>(
            dateTime => dateTime.ToLocalTime(),
            dateTime => dateTime.ToUniversalTime()
        );
    }
}
