using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using Reactive.Bindings.ObjectExtensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class CountNotifierPage : Controls.MyPageControl
    {
        public CountNotifierPage()
        {
            InitializeComponent();
            DataContext = new CountNotifierViewModel();
        }
    }

    class CountNotifierViewModel : MyDisposableBindableBase
    {
        private readonly CountNotifier _countNotifier = new CountNotifier();

        public ReadOnlyReactiveProperty<int> Counter { get; }

        public AsyncReactiveCommand GetResourceCommand1 { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand GetResourceCommand2 { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand GetResourceCommand3 { get; } = new AsyncReactiveCommand();

        public CountNotifierViewModel()
        {
            Counter = _countNotifier
                .Select(_ => _countNotifier.Count)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            GetResourceCommand1
                .Subscribe(async () => await IncrementCountNotifierAsync(1))
                .AddTo(CompositeDisposable);

            GetResourceCommand2
                .Subscribe(async () => await IncrementCountNotifierAsync(2))
                .AddTo(CompositeDisposable);

            GetResourceCommand3
                .Subscribe(async () => await IncrementCountNotifierAsync(3))
                .AddTo(CompositeDisposable);
        }

        private async Task IncrementCountNotifierAsync(int x)
        {
            var d = _countNotifier.Increment(x);
            await Task.Delay(1500);
            d.Dispose();
        }
    }
}
