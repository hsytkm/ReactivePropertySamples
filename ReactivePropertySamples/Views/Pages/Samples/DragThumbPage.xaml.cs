using Microsoft.Xaml.Behaviors;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Interactivity;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class DragThumbPage : Controls.MyPageControl
    {
        public DragThumbPage()
        {
            InitializeComponent();
            DataContext = new DragThumbViewModel();
        }
    }

    class DragThumbViewModel : MyDisposableBindableBase
    {
        public IReactiveProperty<Point> ThumbPoint { get; }
        public IReactiveProperty<Vector> ThumbDragDelta { get; }

        public DragThumbViewModel()
        {
            ThumbPoint = new ReactivePropertySlim<Point>();

            ThumbDragDelta = new ReactivePropertySlim<Vector>(default, ReactivePropertyMode.None);
            ThumbDragDelta.Subscribe(vector =>
            {
                var source = ThumbPoint.Value;
                var x = Math.Clamp(source.X + vector.X, 0, 300);    // 最大側の制限は未実装
                var y = Math.Clamp(source.Y + vector.Y, 0, 200);    // 最大側の制限は未実装
                ThumbPoint.Value = new Point(x, y);
            });
        }
    }

    /// <summary>
    /// Thumb.DragDelta の変化量を Vector で流します
    /// </summary>
    public sealed class ThumbDragDeltaToPointReactiveConverter : ReactiveConverter<DragDeltaEventArgs, Vector>
    {
        protected override IObservable<Vector> OnConvert(IObservable<DragDeltaEventArgs?> source)
        {
            if (AssociateObject is not UIElement uie)
                throw new NotSupportedException("Cannot get mouse events.");

            var mouseDownSequenceSource = Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                h => uie.PreviewMouseDown += h, h => uie.PreviewMouseDown -= h).ToUnit();

            var mouseUpSequence = Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                h => uie.PreviewMouseUp += h, h => uie.PreviewMouseUp -= h).ToUnit();

            // なぜか初回の PreviewDown を拾えないので無理やり流します
            var mouseDownSequence = Observable.Return(Unit.Default).Concat(mouseDownSequenceSource);

            return source
                .Where(x => x is not null)
                .Pairwise()
                .SkipUntil(mouseDownSequence)
                .TakeUntil(mouseUpSequence)
                .Repeat()
                .Select(pair =>
                {
                    (var oldItem, var newItem) = (pair.OldItem!, pair.NewItem!);
                    double x = newItem.HorizontalChange - oldItem.HorizontalChange;
                    double y = newItem.VerticalChange - oldItem.VerticalChange;
                    return new Vector(x, y);
                });
        }
    }

    public sealed class WpfPointToEllipseAction : TriggerAction<Ellipse>
    {
        protected override void Invoke(object parameter)
        {
            if (AssociatedObject is not Ellipse ellipse)
                return;

            if (parameter is not DependencyPropertyChangedEventArgs args)
                return;

            if (args.NewValue is not Point point)
                return;

            const double radius = 5;
            var x = point.X - radius;
            var y = point.Y - radius;
            ellipse.Margin = new Thickness(x, y, 0, 0);
            ellipse.Width = ellipse.Height = radius * 2;
        }
    }
}
