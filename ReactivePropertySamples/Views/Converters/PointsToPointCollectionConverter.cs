using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ReactivePropertySamples.Views.Converters
{
    [ValueConversion(typeof(IEnumerable<Point>), typeof(PointCollection))]
    class PointsToPointCollectionConverter : GenericValueConverter<IEnumerable<Point>, PointCollection>
    {
        public override PointCollection Convert(IEnumerable<Point> points, object parameter, CultureInfo culture) =>
            new PointCollection(points);

        public override IEnumerable<Point> ConvertBack(PointCollection points, object parameter, CultureInfo culture) =>
            points;
    }
}