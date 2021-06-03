#nullable disable
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ReactivePropertySamples.Views.Converters
{
    [ValueConversion(typeof(Color), typeof(Brush))]
    class ColorToBrushConverter : GenericValueConverter<Color, Brush>
    {
        public override Brush Convert(Color color, object parameter, CultureInfo culture)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        public override Color ConvertBack(Brush brush, object parameter, CultureInfo culture) => default;
    }
}
