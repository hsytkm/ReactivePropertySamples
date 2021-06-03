#nullable disable
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
        public CountNotifier Counter { get; } = new CountNotifier();
        public AsyncReactiveCommand GetResourceCommand1 { get; }
        public AsyncReactiveCommand GetResourceCommand2 { get; }
        public AsyncReactiveCommand GetResourceCommand3 { get; }

        public CountNotifierViewModel()
        {
            GetResourceCommand1 = new AsyncReactiveCommand()
                .WithSubscribe(
                    async () => await IncrementCountNotifierAsync(1),
                    CompositeDisposable.Add);

            GetResourceCommand2 = new AsyncReactiveCommand()
                .WithSubscribe(
                    async () => await IncrementCountNotifierAsync(2),
                    CompositeDisposable.Add);

            GetResourceCommand3 = new AsyncReactiveCommand()
                .WithSubscribe(
                    async () => await IncrementCountNotifierAsync(3),
                    CompositeDisposable.Add);
        }

        private async Task IncrementCountNotifierAsync(int x)
        {
            var d = Counter.Increment(x);
            await Task.Delay(1500);
            d.Dispose();
        }
    }
}
