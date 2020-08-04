using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class EventToReactiveProperty2Page : Controls.MyPageControl
    {
        public EventToReactiveProperty2Page()
        {
            InitializeComponent();
            DataContext = new EventToReactiveProperty2ViewModel();
        }
    }

    class EventToReactiveProperty2ViewModel : MyDisposableBindableBase
    {
        public IReactiveProperty<Unit> MouseDownUnit { get; } =
            new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);
        public IReactiveProperty<Unit> MouseUpUnit { get; } =
            new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);
        public IReactiveProperty<Point> MouseMovePoint { get; } =
            new ReactiveProperty<Point>(mode: ReactivePropertyMode.DistinctUntilChanged);

        public IReactiveProperty<Point> DragPoint { get; } = new ReactiveProperty<Point>();

        public EventToReactiveProperty2ViewModel()
        {
            // 前回と最新のマウス位置の差分を流す
            MouseMovePoint
                .Pairwise()
                .Select(x => (x.NewItem - x.OldItem))
                .SkipUntil(MouseDownUnit)
                .TakeUntil(MouseUpUnit)
                .Finally(() => DragPoint.Value = default)
                .Repeat()
                .Subscribe(vec => DragPoint.Value += vec)
                .AddTo(CompositeDisposable);
        }
    }
}
