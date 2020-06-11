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
                .Select(x => !x)
                .ToReactiveCommand()
                .AddTo(CompositeDisposable);

            LightProcessCommand
                .Subscribe(async () =>
                {
                    if (_busyNotifier.IsBusy) return;   // Viewボタンが無効になるので来ない

                    using (_busyNotifier.ProcessStart())
                    {
                        await Task.Delay(500);
                    }
                })
                .AddTo(CompositeDisposable);

            HeavyProcessCommand = _busyNotifier
                .Select(x => !x)
                .ToReactiveCommand()
                .AddTo(CompositeDisposable);

            HeavyProcessCommand
                .Subscribe(async () =>
                {
                    if (_busyNotifier.IsBusy) return;   // Viewボタンが無効になるので来ない

                    using (_busyNotifier.ProcessStart())
                    {
                        await Task.Delay(2000);
                    }
                })
                .AddTo(CompositeDisposable);
        }
    }
}
