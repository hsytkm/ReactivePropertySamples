using Reactive.Bindings;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class AsyncReactiveCommand1Page : Controls.MyPageControl
    {
        public AsyncReactiveCommand1Page()
        {
            InitializeComponent();
            DataContext = new AsyncReactiveCommand1ViewModel();
        }
    }

    class AsyncReactiveCommand1ViewModel : MyDisposableBindableBase
    {
        public AsyncReactiveCommand ButtonClickAsyncCommand1 { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand ButtonClickAsyncCommand2 { get; } = new AsyncReactiveCommand();

        public AsyncReactiveCommand1ViewModel()
        {
            ButtonClickAsyncCommand1.Subscribe(async _ =>
            {
                await Task.Delay(1000);
            });

            ButtonClickAsyncCommand2.Subscribe(async _ =>
            {
                await Task.Delay(3000);
            });
        }
    }
}
