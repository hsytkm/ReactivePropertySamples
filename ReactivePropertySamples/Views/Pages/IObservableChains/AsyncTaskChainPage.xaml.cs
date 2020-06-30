using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class AsyncTaskChainPage : Controls.MyPageControl
    {
        public AsyncTaskChainPage()
        {
            InitializeComponent();
        }
    }

    class AsyncTaskChainViewModel : MyDisposableBindableBase
    {
        public IReadOnlyReactiveProperty<DateTime> TimeNow { get; }

        public ReactiveCommand<Color> SetColor1Command { get; }
        public ReactiveCommand<Color> SetColor2Command { get; }
        public ReactiveCommand<Color> SetColor3Command { get; }
        public IReadOnlyReactiveProperty<SolidColorBrush> FillBrush1 { get; }
        public IReadOnlyReactiveProperty<SolidColorBrush> FillBrush2 { get; }
        public IReadOnlyReactiveProperty<SolidColorBrush> FillBrush3 { get; }

        public AsyncTaskChainViewModel()
        {
            TimeNow = Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Select(_ => DateTime.Now)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            /* 1: UIフリーズする */
            var model1 = new AsyncTaskChainModel();
            SetColor1Command = new ReactiveCommand<Color>()
                .WithSubscribe(x => model1.UserColor = x, CompositeDisposable.Add);

            FillBrush1 = model1.ObserveProperty(x => x.UserColor, isPushCurrentValueAtFirst: false)
                .Select(color => ColorToBrushAsync(color))
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);


            /* 2: UIフリーズしない(Task) */
            var model2 = new AsyncTaskChainModel();
            SetColor2Command = new ReactiveCommand<Color>()
                .WithSubscribe(x => model2.UserColor = x, CompositeDisposable.Add);

            // Select 内で Task を使用して、IObservable<T> を流す
            // the second call will start only when the first one is already finished.
            FillBrush2 = model2.ObserveProperty(x => x.UserColor, isPushCurrentValueAtFirst: false)
                .Select(color => Observable.FromAsync(() => Task.Run(() => ColorToBrushAsync(color))))
                .Concat()
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);


            /* 3: UIフリーズしない(Scheduler) */
            var model3 = new AsyncTaskChainModel();
            SetColor3Command = new ReactiveCommand<Color>()
                .WithSubscribe(x => model3.UserColor = x, CompositeDisposable.Add);

            // the second call to the method could start before the end of the first call.
            FillBrush3 = model3.ObserveProperty(x => x.UserColor, isPushCurrentValueAtFirst: false)
                .ObserveOn(Scheduler.Default)
                .Select(color => ColorToBrushAsync(color))
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
        }

        private static SolidColorBrush ColorToBrushAsync(Color color)
        {
            Thread.Sleep(1500);     // 重い処理想定のウェイト（現スレッド）

            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }
    }

    class AsyncTaskChainModel : MyBindableBase
    {
        public Color UserColor
        {
            get => _userColor;
            set => SetProperty(ref _userColor, value);
        }
        private Color _userColor;
    }
}
