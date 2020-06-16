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
    public partial class MessageBroker1Page : Controls.MyPageControl
    {
        public MessageBroker1Page()
        {
            InitializeComponent();
            DataContext = new MessageBroker1ViewModel();
        }
    }

    class MessagePayload1
    {
        public string Message { get; }
        public MessagePayload1(string msg) => Message = msg;
    }

    class MessageBroker1ViewModel : MyDisposableBindableBase
    {
        public ReactiveCollection<MessagePayload1> MessageValues1 { get; } =
            new ReactiveCollection<MessagePayload1>();
        public ReactiveCollection<MessagePayload1> MessageValues2 { get; } =
            new ReactiveCollection<MessagePayload1>();

        public ReactiveCommand<string> SendMessageCommand { get; }
        public ReactiveCommand UnsubscribeCommand { get; }

        public MessageBroker1ViewModel()
        {
            // 0. 1度だけ Dispose できるボタンを制御するための Rp
            var disposableProperty = new ReactiveProperty<IDisposable>()
                .AddTo(CompositeDisposable);

            // 1-1. MessageBroker(Singlton) への登録
            //      T型 が Publish されたら Viewコレクション1 を更新
            disposableProperty.Value = MessageBroker.Default
                .Subscribe<MessagePayload1>(msg => MessageValues1.AddOnScheduler(msg))
                .AddTo(CompositeDisposable);

            // 1-2. MessageBroker(Singlton) への登録
            //      T型 が Publish されたら Viewコレクション2 を更新 (こちらは Dispose されない)
            MessageBroker.Default
                .Subscribe<MessagePayload1>(msg => MessageValues2.AddOnScheduler(msg))
                .AddTo(CompositeDisposable);

            // 2. Command をトリガに、T型 を Publish
            SendMessageCommand = new ReactiveCommand<string>()
                .WithSubscribe(s =>
                    MessageBroker.Default.Publish(new MessagePayload1(s)),
                    CompositeDisposable.Add);

            // 3. 一度だけ Dispose(Unsubscribe)できるCommand
            UnsubscribeCommand = disposableProperty
                .Select(x => x != null)
                .ToReactiveCommand()
                .AddTo(CompositeDisposable);
            UnsubscribeCommand
                .Subscribe(() =>
                {
                    disposableProperty.Value?.Dispose();
                    disposableProperty.Value = null;
                })
                .AddTo(CompositeDisposable);
        }
    }
}
