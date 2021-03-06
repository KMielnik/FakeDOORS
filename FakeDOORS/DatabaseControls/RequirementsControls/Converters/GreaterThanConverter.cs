﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace FakeDOORS.DatabaseControls.RequirementsControls.Converters
{
    class GreaterThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value) > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
