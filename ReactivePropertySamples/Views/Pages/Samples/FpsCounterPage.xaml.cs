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
        public IReadOnlyReactiveProperty<double> FpsCounter3 { get; }

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
                .TimeInterval()
                .Buffer(count: _bufferLength, skip: 1)
                .Select(xs => 1000d / xs.Select(x => x.Interval.Milliseconds).Average())
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);

            FpsCounter3 = MouseDownUnit
                .Framerate(_bufferLength)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);
        }
    }

    static class MyObservableFpsExtension
    {
        /// <summary>
        /// Observable シーケンスのフレームレート(fps)を計算します。
        /// </summary>
        /// <param name="source">元シーケンス</param>
        /// <param name="bufferCount">フレームレートを計算するためのバッファ数</param>
        /// <returns>fps数(異常時は0)</returns>
        public static IObservable<double> Framerate(this IObservable<Unit> source, int bufferCount)
            => source.Select(_ => DateTime.Now).Framerate(bufferCount);

        /// <summary>
        /// Observable シーケンスのフレームレート(fps)を計算します。
        /// </summary>
        /// <param name="source">元シーケンス</param>
        /// <param name="bufferCount">フレームレートを計算するためのバッファ数</param>
        /// <returns>fps数(異常時は0)</returns>
        private static IObservable<double> Framerate(this IObservable<DateTime> source, int bufferCount)
        {
            return Observable.Create<double>(observer =>
            {
                var bufferMSec = new List<double>(Math.Max(2, bufferCount));  // count minimum = 2
                (bool isFirst, DateTime time) prev = (true, default);

                return source.Subscribe(time =>
                {
                    if (prev.isFirst)
                    {
                        prev = (false, time);
                        return;
                    }

                    var msec = (time - prev.time).TotalMilliseconds;
                    prev.time = time;

                    if (bufferMSec.Count >= bufferMSec.Capacity)
                        bufferMSec.RemoveAt(0);

                    bufferMSec.Add(msec);

                    if (bufferMSec.Count < bufferMSec.Capacity)
                        return;

                    var ave = bufferMSec.Average();
                    var fps = (ave > 0) ? 1000d / ave : 0;

                    observer.OnNext(fps);
                }, observer.OnError, observer.OnCompleted);
            });
        }
    }
}
