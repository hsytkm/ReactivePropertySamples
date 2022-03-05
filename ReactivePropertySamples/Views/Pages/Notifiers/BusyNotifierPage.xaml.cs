using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class BusyNotifierPage : Controls.MyPageControl
    {
        public BusyNotifierPage()
        {
            InitializeComponent();
            DataContext = new BusyNotifierViewModel();
        }
    }

    class BusyNotifierViewModel : MyDisposableBindableBase
    {
        private readonly BusyNotifier _busyNotifier = new BusyNotifier();
        public ReactiveCommand LightProcessCommand { get; }
        public ReactiveCommand HeavyProcessCommand { get; }

        public BusyNotifierViewModel()
        {
            LightProcessCommand = _busyNotifier
                .Inverse()
                .ToReactiveCommand()
                .WithSubscribe(async () =>
                {
                    // Viewボタンが無効になるので来ないが勉強のため
                    if (_busyNotifier.IsBusy) throw new Exception();

                    using (_busyNotifier.ProcessStart())
                    {
                        await Task.Delay(500);
                    }
                }, CompositeDisposable.Add)
                .AddTo(CompositeDisposable);

            HeavyProcessCommand = _busyNotifier
                .Inverse()
                .ToReactiveCommand()
                .WithSubscribe(async () =>
                {
                    // Viewボタンが無効になるので来ないが勉強のため
                    if (_busyNotifier.IsBusy) throw new Exception();

                    using (_busyNotifier.ProcessStart())
                    {
                        await Task.Delay(2000);
                    }
                }, CompositeDisposable.Add)
                .AddTo(CompositeDisposable);
        }
    }
}
