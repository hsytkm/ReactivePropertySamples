using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class FpsCounterPage : Controls.MyPageControl
    {
        public FpsCounterPage()
        {
            InitializeComponent();
            DataContext = new FpsCounterViewModel();
        }
    }

    class FpsCounterViewModel : MyDisposableBindableBase
    {
        public IReactiveProperty<Unit> MouseDownUnit { get; }

        private const int _bufferLength = 6;
        public IReadOnlyReactiveProperty<double> FpsCounter1 { get; }
        public IReadOnlyReactiveProperty<double> FpsCounter2 { get; }

        public FpsCounterViewModel()
        {
            MouseDownUnit = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None).AddTo(CompositeDisposable);

            // http://reactivex.io/documentation/operators/buffer.html
            // Buffer(count, skip) : count個のデータが溜まったら、skip個ごとに count個をまとめた List<T> を流します
            // Buffer(count) : countの倍数で、count個をまとめた List<T> を流します。
            FpsCounter1 = MouseDownUnit
                .Select(_ => DateTime.Now)
                .Pairwise()
                .Select(dateTime => dateTime.NewItem - dateTime.OldItem)
                .Buffer(count: _bufferLength, skip: 1)
                .Select(tss => 1000d / tss.Select(ts => ts.TotalMilliseconds).Average())
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);

            FpsCounter2 = MouseDownUnit
                .Framerate(_bufferLength)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);

        }
    }

    static class MyObservableFpsExtension
    {
        public static IObservable<double> Framerate(this IObservable<Unit> source, int count)
            => source.Select(_ => DateTime.Now).Framerate(count);

        public static IObservable<double> Framerate(this IObservable<DateTime> source, int count)
        {
            var result = Observable.Create<double>(observer =>
            {
                var bufferMSec = new List<double>(Math.Max(2, count));  // count minimum = 2
                (bool isFirst, DateTime time) prev = (true, default);

                return source.Subscribe(time =>
                {
                    if (prev.isFirst)
                    {
                        prev.time = time;
                        prev.isFirst = false;
                        return;
                    }

                    var msec = (time - prev.time).TotalMilliseconds;
                    prev.time = time;

                    if (bufferMSec.Count < bufferMSec.Capacity)
                    {
                        bufferMSec.Add(msec);
                        return;
                    }

                    bufferMSec.RemoveAt(0);
                    bufferMSec.Add(msec);

                    var ave = bufferMSec.Average();
                    var fps = 0d;
                    if (ave > 0)
                    {
                        fps = 1000d / ave;
                    }

                    observer.OnNext(fps);
                }, observer.OnError, observer.OnCompleted);
            });
            return result;
        }
    }
}
