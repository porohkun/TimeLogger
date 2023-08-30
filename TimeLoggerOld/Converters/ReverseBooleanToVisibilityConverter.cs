﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace TimeLoggerOld
{
    public sealed class ReverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is bool)
                flag = (bool)value;
            else if (value is bool?)
            {
                bool? nullable = (bool?)value;
                flag = nullable.HasValue && nullable.Value;
            }
            return (object)(Visibility)(flag ? 2 : 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
                return (object)((Visibility)value != Visibility.Visible);
            return (object)true;
        }
    }
}
