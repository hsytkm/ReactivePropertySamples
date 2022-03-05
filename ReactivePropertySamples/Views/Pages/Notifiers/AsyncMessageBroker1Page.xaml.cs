#nullable disable
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class AsyncMessageBroker1Page : Controls.MyPageControl
    {
        public AsyncMessageBroker1Page()
        {
            InitializeComponent();
            DataContext = new AsyncMessageBroker1ViewModel();
        }
    }

    class AsyncMessagePayload1
    {
        public string Message { get; }
        public AsyncMessagePayload1(string msg) => Message = msg;
    }

    class AsyncMessageBroker1ViewModel : MyDisposableBindableBase
    {
        public ReactiveCollection<MessagePayload1> MessageValues1 { get; } =
            new ReactiveCollection<MessagePayload1>();
        public ReactiveCollection<MessagePayload1> MessageValues2 { get; } =
            new ReactiveCollection<MessagePayload1>();

        public AsyncReactiveCommand<string> SendMessageCommand { get; }
        public ReactiveCommand UnsubscribeCommand { get; }

        public AsyncMessageBroker1ViewModel()
        {
            // 0. 1度だけ Dispose できるボタンを制御するための Rp
            var disposableProperty = new ReactiveProperty<IDisposable>()
                .AddTo(CompositeDisposable);

            // 1-1. AsyncMessageBroker(Singlton) への登録
            //      T型 が Publish されたら Viewコレクション1 を更新
            disposableProperty.Value = AsyncMessageBroker.Default
                .Subscribe<MessagePayload1>(async msg =>
                {
                    await Task.Delay(1000);
                    MessageValues1.AddOnScheduler(msg);
                })
                .AddTo(CompositeDisposable);

            // 1-2. AsyncMessageBroker(Singlton) への登録
            //      T型 が Publish されたら Viewコレクション2 を更新 (こちらは Dispose されない)
            AsyncMessageBroker.Default
                .Subscribe<MessagePayload1>(async msg =>
                {
                    await Task.Delay(2000);
                    MessageValues2.AddOnScheduler(msg);
                })
                .AddTo(CompositeDisposable);

            // 2. AsyncCommand をトリガに、T型 を PublishAsync
            SendMessageCommand = new AsyncReactiveCommand<string>()
                .WithSubscribe(async s =>
                    await AsyncMessageBroker.Default.PublishAsync(new MessagePayload1(s)),
                    CompositeDisposable.Add)
                .AddTo(CompositeDisposable);

            // 3. 一度だけ Dispose(Unsubscribe)できるCommand
            UnsubscribeCommand = disposableProperty
                .Select(x => x is not null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    disposableProperty.Value?.Dispose();
                    disposableProperty.Value = null;
                })
                .AddTo(CompositeDisposable);
        }
    }
}
