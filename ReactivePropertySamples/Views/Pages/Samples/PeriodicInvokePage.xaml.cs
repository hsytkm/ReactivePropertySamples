using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class PeriodicInvokePage : Controls.MyPageControl
    {
        public PeriodicInvokePage()
        {
            InitializeComponent();
            DataContext = new PeriodicInvokeViewModel();
        }
    }

    class PeriodicInvokeViewModel : MyDisposableBindableBase
    {
        private int _counter;

        public IReactiveProperty<int> HeavyProcessInvokeTimeMsec { get; }
        public IReadOnlyReactiveProperty<int> InvokeCounter { get; }
        public IReadOnlyReactiveProperty<DateTime> DateTimeNow { get; }

        public PeriodicInvokeViewModel()
        {
            HeavyProcessInvokeTimeMsec = new ReactivePropertySlim<int>(initialValue: 1000).AddTo(CompositeDisposable);

            InvokeCounter = Observable.FromAsync(() => HeavyTaskAsync(HeavyProcessInvokeTimeMsec.Value))
                .Zip(Observable.Timer(TimeSpan.FromMilliseconds(500)), (x, _) => x)
                .Repeat()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);

            DateTimeNow = Observable.Timer(TimeSpan.FromMilliseconds(100))
                .Repeat()
                .Select(_ => DateTime.Now)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);
        }

        private async Task<int> HeavyTaskAsync(int waitMsec)
        {
            await Task.Delay(waitMsec);
            return ++_counter;
        }
    }
}
