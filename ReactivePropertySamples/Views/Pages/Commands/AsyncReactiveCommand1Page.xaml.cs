using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
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
        public AsyncReactiveCommand ButtonClickAsyncCommand1 { get; }
        public AsyncReactiveCommand ButtonClickAsyncCommand2 { get; }

        public AsyncReactiveCommand1ViewModel()
        {
            // インスタンス作ってから Subscribe するパターン
            ButtonClickAsyncCommand1 = new AsyncReactiveCommand()
                .AddTo(CompositeDisposable);
            ButtonClickAsyncCommand1
                .Subscribe(async () => await Task.Delay(1000))
                .AddTo(CompositeDisposable);

            // WithSubscribe() : インスタンス化 + Disposable 登録
            ButtonClickAsyncCommand2 = new AsyncReactiveCommand()
                .WithSubscribe(async () => await Task.Delay(3000), CompositeDisposable.Add);
        }
    }
}
