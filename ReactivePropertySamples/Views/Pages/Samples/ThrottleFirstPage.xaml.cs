using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ThrottleFirstPage : Controls.MyPageControl
    {
        public ThrottleFirstPage()
        {
            DataContext = new ThrottleFirstViewModel();
            InitializeComponent();
        }
    }

    class ThrottleFirstViewModel : MyDisposableBindableBase
    {
        public IReactiveProperty<string> InputText { get; }
        public IReadOnlyReactiveProperty<string> ThrottleText { get; }
        public IReadOnlyReactiveProperty<string> SampleText { get; }
        public IReadOnlyReactiveProperty<string> ThrottleFirstText { get; }

        public ThrottleFirstViewModel()
        {
            var timeSpan = TimeSpan.FromMilliseconds(2000);
            InputText = new ReactivePropertySlim<string>(
                    initialValue: "", mode: ReactivePropertyMode.DistinctUntilChanged)
                .AddTo(CompositeDisposable);

            // 入力終了から4秒したらクリアする
            InputText
                .Sample(TimeSpan.FromMilliseconds(4000))
                .Subscribe(_ => InputText.Value = "")
                .AddTo(CompositeDisposable);

            // 入力文字を1文字ずつ流す
            var inputChar = InputText
                .Select(x => x.Length > 0 ? x[^1] : default(char?))
                .Where(c => c is not null)
                .ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.None)
                .AddTo(CompositeDisposable);

            // 最後に値が流れてきてから2秒経過したら次に流します
            ThrottleText = inputChar
                .Throttle(timeSpan)
                .Scan("", (concatString, latestChar) => string.Concat(concatString, latestChar.ToString()))
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);

            // 値が流れてきてから2秒経過した時点の値を流します
            SampleText = inputChar
                .Sample(timeSpan)
                .Scan("", (concatString, latestChar) => string.Concat(concatString, latestChar.ToString()))
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);

            // 値を直ちに流しますが、次は2秒経過するまでは流しません
            ThrottleFirstText = inputChar
                .ThrottleFirst(timeSpan)
                .Scan("", (concatString, latestChar) => string.Concat(concatString, latestChar.ToString()))
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);
        }
    }

    static class MyObservableThrottleFirstExtension
    {
        // see https://github.com/dotnet/reactive/issues/395

        public static IObservable<T> ThrottleFirst<T>(this IObservable<T> source, TimeSpan delay)
        {
            return source.Publish(o =>
            {
                return o.Take(1)
                    .Concat(o.IgnoreElements().TakeUntil(Observable.Return(default(T)).Delay(delay)))
                    .Repeat()
                    .TakeUntil(o.IgnoreElements().Concat(Observable.Return(default(T))));
            });
        }
    }
}
