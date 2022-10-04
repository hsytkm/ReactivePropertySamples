using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class EventToReactiveProperty5Page : Controls.MyPageControl
    {
        public EventToReactiveProperty5Page()
        {
            InitializeComponent();
            DataContext = new EventToReactiveProperty5ViewModel();
        }
    }

    class EventToReactiveProperty5ViewModel : MyDisposableBindableBase
    {
        public MouseClickListener MouseLeftButton { get; }
        public IReadOnlyReactiveProperty<string> Message { get; }

        public EventToReactiveProperty5ViewModel()
        {
            MouseLeftButton = new MouseClickListener().AddTo(CompositeDisposable);

            Message = MouseLeftButton.Clicking
                .Select(b => $"Mouse Left Click = {b}")
                .ToReadOnlyReactivePropertySlim<string>()
                .AddTo(CompositeDisposable);
        }
    }
}

sealed class MouseClickListener : MyDisposableBindableBase
{
    public IReactiveProperty<Unit> DownPoint { get; }
    public IReactiveProperty<Unit> UpPoint { get; }
    public IReadOnlyReactiveProperty<bool> Clicking { get; }

    public MouseClickListener()
    {
        DownPoint = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None).AddTo(CompositeDisposable);
        UpPoint = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None).AddTo(CompositeDisposable);

        Clicking = Observable.Merge(DownPoint.Select(_ => true), UpPoint.Select(_ => false))
            .ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.DistinctUntilChanged).AddTo(CompositeDisposable);

        //Clicking.Subscribe(b => System.Diagnostics.Debug.WriteLine($"Clicking: {b}")).AddTo(CompositeDisposable);
    }
}
