using System;
using System.Windows.Data;

namespace TimeLogger.Converters
{
    public sealed class SelectValueByBoolConverter : IMultiValueConverter
    {
        public object? Convert(object?[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 3)
                return null;
            var checker = (values[0] as bool?) ?? false;
            return (checker ? values[1] : values[2])?.ToString();
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("SelectValueByBoolConverter is a one way converter.");
        }
    }
}
