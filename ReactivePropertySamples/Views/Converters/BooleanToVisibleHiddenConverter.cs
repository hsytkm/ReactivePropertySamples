using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReactivePropertySamples.Views.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class BooleanToVisibleHiddenConverter : IValueConverter
    {
        // 標準の BooleanToVisibilityConverter の false は Collapsed なので作成
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) throw new ArgumentNullException();
            if (value is bool b) return b ? Visibility.Visible : Visibility.Hidden;
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => new NotImplementedException();
    }
}
