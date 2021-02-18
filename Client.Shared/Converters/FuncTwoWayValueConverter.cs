using System;
using System.Globalization;

using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Utilities;

namespace Client.Shared.Converters
{
    public class FuncTwoWayValueConverter<TIn, TOut> : IValueConverter
    {
        private readonly Func<TIn, TOut> _convert;
        private readonly Func<TOut, TIn> _convertBack;

        public FuncTwoWayValueConverter(Func<TIn, TOut> convert, Func<TOut, TIn> convertBack)
        {
            _convert = convert;
            _convertBack = convertBack;
        }

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TIn || value == null && TypeUtilities.AcceptsNull(typeof(TIn)))
            {
                return _convert((TIn)value);
            }

            return AvaloniaProperty.UnsetValue;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TOut || value == null && TypeUtilities.AcceptsNull(typeof(TOut)))
            {
                return _convertBack((TOut)value);
            }

            return AvaloniaProperty.UnsetValue;
        }
    }
}
