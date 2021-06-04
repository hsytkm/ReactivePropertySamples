using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace ReactivePropertySamples.Views.Converters
{
    [ValueConversion(typeof(IEnumerable<string>), typeof(string))]
    class StringConcatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) throw new ArgumentNullException();
            if (value is string s) return s;
            if (value is IEnumerable<string> ss) return string.Join(" ", ss);
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => new NotImplementedException();
    }
}
