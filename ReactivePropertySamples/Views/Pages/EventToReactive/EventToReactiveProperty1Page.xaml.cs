#nullable disable
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
    public partial class EventToReactiveProperty1Page : Controls.MyPageControl
    {
        public EventToReactiveProperty1Page()
        {
            InitializeComponent();
            DataContext = new EventToReactiveProperty1ViewModel();
        }
    }

    class MouseEventCounter : MyBindableBase
    {
        public int PreviewMouseDownCounter { get; set; }
        public int MouseDownCounter { get; set; }
        public int MouseUpCounter { get; set; }
        public int PreviewMouseUpCounter { get; set; }
    }

    class EventToReactiveProperty1ViewModel : MyDisposableBindableBase
    {
        public MouseEventCounter MouseEventCounter { get; } = new MouseEventCounter();
        public IReactiveProperty<Unit> PreviewMouseDownUnit { get; } =
            new ReactiveProperty<Unit>(mode:ReactivePropertyMode.None);
        public IReactiveProperty<Unit> MouseDownUnit { get; } =
            new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);
        public IReactiveProperty<Unit> MouseUpUnit { get; } =
            new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);
        public IReactiveProperty<Unit> PreviewMouseUpUnit { get; } =
            new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);

        public IReactiveProperty<Point> MouseMovePoint1 { get; } = new ReactiveProperty<Point>();
        public IReactiveProperty<Point> MouseMovePoint2 { get; } = new ReactiveProperty<Point>();

        public EventToReactiveProperty1ViewModel()
        {
            PreviewMouseDownUnit
                .Select(_ => nameof(PreviewMouseDownUnit).Replace("Unit", ""))
                .Subscribe(x =>
                {
                    MouseEventCounter.PreviewMouseDownCounter++;
                    NotifyPropertyChanged(nameof(MouseEventCounter));
                })
                .AddTo(CompositeDisposable);

            MouseDownUnit
                .Select(_ => nameof(MouseDownUnit).Replace("Unit", ""))
                .Subscribe(x =>
                {
                    MouseEventCounter.MouseDownCounter++;
                    NotifyPropertyChanged(nameof(MouseEventCounter));
                })
                .AddTo(CompositeDisposable);

            MouseUpUnit
                .Select(_ => nameof(MouseUpUnit).Replace("Unit", ""))
                .Subscribe(x =>
                {
                    MouseEventCounter.MouseUpCounter++;
                    NotifyPropertyChanged(nameof(MouseEventCounter));
                })
                .AddTo(CompositeDisposable);

            PreviewMouseUpUnit
                .Select(_ => nameof(PreviewMouseUpUnit).Replace("Unit", ""))
                .Subscribe(x =>
                {
                    MouseEventCounter.PreviewMouseUpCounter++;
                    NotifyPropertyChanged(nameof(MouseEventCounter));
                })
                .AddTo(CompositeDisposable);
        }

    }
}
