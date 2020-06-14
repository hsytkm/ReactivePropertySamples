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
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class MessageBrokerPage : Controls.MyPageControl
    {
        public MessageBrokerPage()
        {
            InitializeComponent();
            DataContext = new MessageBrokerViewModel();
        }
    }

    class SampleClass
    {
        public int MyProperty { get; set; }
        public override string ToString() => $"SampleClass.MyProperty = {MyProperty}";
    }

    class MessageBrokerViewModel : MyDisposableBindableBase
    {
        public MessageBrokerViewModel()
        {
            Debug.WriteLine("MessageBroker");
            //RunMessageBroker();

            //Debug.WriteLine("AsyncMessageBroker");
            //Task.Run(async () => await RunMessageBrokerAsync());
        }

        private void RunMessageBroker()
        {
            // global scope pub-sub messaging
            MessageBroker.Default
                .Subscribe<SampleClass>(x => Debug.WriteLine("A:" + x))
                .AddTo(CompositeDisposable);

            // support convert to IObservable<T>
            using (MessageBroker.Default
                    .ToObservable<SampleClass>()
                    .Subscribe(x => Debug.WriteLine("B:" + x)))
            {
                MessageBroker.Default.Publish(new SampleClass { MyProperty = 1 });
                MessageBroker.Default.Publish(new SampleClass { MyProperty = 2 });
            }

            MessageBroker.Default.Publish(new SampleClass { MyProperty = 3 });
        }

        private async Task RunMessageBrokerAsync()
        {
            // asynchronous message pub-sub
            AsyncMessageBroker.Default
                .Subscribe<SampleClass>(async x =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    Debug.WriteLine("C:" + x);
                })
                .AddTo(CompositeDisposable);

            using (AsyncMessageBroker.Default
                    .Subscribe<SampleClass>(async x =>
                {
                    Console.WriteLine("D:" + x);
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }))
            {
                // await all subscriber complete
                await AsyncMessageBroker.Default.PublishAsync(new SampleClass { MyProperty = 10 });
                await AsyncMessageBroker.Default.PublishAsync(new SampleClass { MyProperty = 20 });
            }

            await AsyncMessageBroker.Default.PublishAsync(new SampleClass { MyProperty = 30 });
        }

    }
}
