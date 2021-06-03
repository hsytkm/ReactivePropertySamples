#nullable disable
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ReactivePropertySamples.Views.Converters
{
    [ValueConversion(typeof(Rect), typeof(PointCollection))]
    class RectToPointCollectionConverter : GenericValueConverter<Rect, PointCollection>
    {
        public override PointCollection Convert(Rect rect, object parameter, CultureInfo culture)
            => new PointCollection(new[] { rect.TopLeft, rect.TopRight, rect.BottomRight, rect.BottomLeft });

        public override Rect ConvertBack(PointCollection points, object parameter, CultureInfo culture)
            => Rect.Empty;
    }
}