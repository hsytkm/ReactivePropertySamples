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
    public partial class AsyncTaskSelectPage : Controls.MyPageControl
    {
        public AsyncTaskSelectPage()
        {
            InitializeComponent();
            //DataContext = new AsyncTaskSelectViewModel();
        }
    }

    class AsyncTaskSelectViewModel : MyDisposableBindableBase
    {
        public ReadOnlyReactiveProperty<DateTime> TimeNow { get; }

        private readonly AsyncTaskSelectModel _model1 = new AsyncTaskSelectModel();
        private readonly AsyncTaskSelectModel _model2 = new AsyncTaskSelectModel();
        public ReactiveCommand<Color> SetColor1Command { get; } = new ReactiveCommand<Color>();
        public ReactiveCommand<Color> SetColor2Command { get; } = new ReactiveCommand<Color>();
        public ReadOnlyReactiveProperty<SolidColorBrush> FillBrush1 { get; }
        public ReadOnlyReactiveProperty<SolidColorBrush> FillBrush2 { get; }


        public AsyncTaskSelectViewModel()
        {
            TimeNow = Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Select(_ => DateTime.Now)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            SetColor1Command
                .Subscribe(x => _model1.UserColor = x)
                .AddTo(CompositeDisposable);

            FillBrush1 = _model1.ObserveProperty(x => x.UserColor, isPushCurrentValueAtFirst: false)
                .Select(color => ColorToBrushAsync(color))
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);


            SetColor2Command
                .Subscribe(x => _model2.UserColor = x)
                .AddTo(CompositeDisposable);

            // Select 内で Task を使用して、IObservable<T> を流す
            FillBrush2 = _model2.ObserveProperty(x => x.UserColor, isPushCurrentValueAtFirst: false)
                .Select(color => Observable.FromAsync(() => Task.Run(() => ColorToBrushAsync(color))))
                .Concat()
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
        }

        private static SolidColorBrush ColorToBrushAsync(Color color)
        {
            Thread.Sleep(1500);

            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }
    }

    class AsyncTaskSelectModel : MyBindableBase
    {
        public Color UserColor
        {
            get => _userColor;
            set => SetProperty(ref _userColor, value);
        }
        private Color _userColor;
    }
}
