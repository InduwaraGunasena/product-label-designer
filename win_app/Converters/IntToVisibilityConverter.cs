using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace win_app.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            int intValue = System.Convert.ToInt32(value);
            int targetValue = System.Convert.ToInt32(parameter);

            return intValue == targetValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
