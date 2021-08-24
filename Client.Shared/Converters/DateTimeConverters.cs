using System;

using Avalonia.Data.Converters;

namespace Client.Shared.Converters
{
    static public class DateTimeConverters
    {
        static public readonly IValueConverter UtcToLocal = new FuncTwoWayValueConverter<DateTime, DateTime>(
            convert: dateTime => dateTime.ToLocalTime(),
            convertBack: dateTime => dateTime.ToUniversalTime()
        );
    }
}
