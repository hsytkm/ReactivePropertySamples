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

    class SampleMessage1
    {
        public int IntValue { get; set; }
        public override string ToString() => $"SampleMessage1.IntValue = {IntValue}";
    }
    class SampleMessage2
    {
        public double DoubleValue { get; set; }
        public override string ToString() => $"SampleMessage2.DoubleValue = {DoubleValue}";
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
                .Subscribe<SampleMessage1>(x => Debug.WriteLine("Sample1-A:" + x))
                .AddTo(CompositeDisposable);

            MessageBroker.Default
                .Subscribe<SampleMessage2>(x => Debug.WriteLine("Sample2-A:" + x))
                .AddTo(CompositeDisposable);

            // support convert to IObservable<T>
            var d = MessageBroker.Default
                    .ToObservable<SampleMessage1>()
                    .Subscribe(x => Debug.WriteLine("Sample1-B:" + x));

            MessageBroker.Default.Publish(new SampleMessage1 { IntValue = 1 });
            MessageBroker.Default.Publish(new SampleMessage2 { DoubleValue = 2.2 });
            d.Dispose();

            MessageBroker.Default.Publish(new SampleMessage1 { IntValue = 3 });
        }

        private async Task RunMessageBrokerAsync()
        {
            // asynchronous message pub-sub
            AsyncMessageBroker.Default
                .Subscribe<SampleMessage1>(async x =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    Debug.WriteLine("C:" + x);
                })
                .AddTo(CompositeDisposable);

            using (AsyncMessageBroker.Default
                    .Subscribe<SampleMessage1>(async x =>
                {
                    Console.WriteLine("D:" + x);
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }))
            {
                // await all subscriber complete
                await AsyncMessageBroker.Default.PublishAsync(new SampleMessage1 { IntValue = 10 });
                await AsyncMessageBroker.Default.PublishAsync(new SampleMessage1 { IntValue = 20 });
            }

            await AsyncMessageBroker.Default.PublishAsync(new SampleMessage1 { IntValue = 30 });
        }

    }
}
