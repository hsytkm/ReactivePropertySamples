using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

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
        public ICommand ButtonClickAsyncCommand2 { get; }

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
                .WithSubscribe(async () => await Task.Delay(3000), CompositeDisposable.Add)
                .AddTo(CompositeDisposable);

            // IObservable<bool>から作成する例
            var boolean = new BooleanNotifier();
            var command = boolean
                .ToAsyncReactiveCommand()
                .WithSubscribe(() => Task.CompletedTask, CompositeDisposable.Add)
                .AddTo(CompositeDisposable);
        }
    }
}
