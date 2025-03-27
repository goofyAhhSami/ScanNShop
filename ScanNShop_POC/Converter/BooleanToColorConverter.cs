using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace ScanNShop_POC.Converter
{
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked)
            {
                return isChecked ? Color.FromArgb("#FF8A80") : Color.FromArgb("#A8E6CF");
            }
            return Color.FromArgb("#A8E6CF");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
