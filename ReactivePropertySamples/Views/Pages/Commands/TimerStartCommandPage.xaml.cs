using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class TimerStartCommandPage : Controls.MyPageControl
    {
        public TimerStartCommandPage()
        {
            InitializeComponent();
            DataContext = new TimerStartCommandViewModel();
        }
    }

    class TimerStartCommandViewModel : MyDisposableBindableBase
    {
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public IReadOnlyReactiveProperty<TimeSpan> MyTimer1 { get; }
        public IReadOnlyReactiveProperty<TimeSpan> MyTimer2 { get; }

        public TimerStartCommandViewModel()
        {
            // ctorからカウント開始
            var ctorDateTime = DateTime.Now;
            MyTimer1 = Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(_ => DateTime.Now - ctorDateTime)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);

#if false
            // 1. DataTime.Now からの差分でカウント  ◆もう少しかっちょよい実装ない？
            var clickDateTime = DateTime.MinValue;
            MyTimer2 = Observable.Interval(TimeSpan.FromSeconds(1))
                .Where(_ => clickDateTime != DateTime.MinValue)
                .Select(_ => DateTime.Now - clickDateTime)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);

            StartCommand = new ReactiveCommand()
                .WithSubscribe(() => clickDateTime = DateTime.Now, CompositeDisposable.Add);

            StopCommand = new ReactiveCommand()
                .WithSubscribe(() => clickDateTime = DateTime.MinValue, CompositeDisposable.Add);

#else
            // 2. BooleanNotifier で管理。Command.CanExecute も取れるので良い感じ
            var timerRunning = new BooleanNotifier();       // BooleanNotifier は IDisposable じゃないので良い！
            //var timerRunning = new ReactivePropertySlim<bool>(initialValue: false).AddTo(CompositeDisposable);

            MyTimer2 = Observable.Interval(TimeSpan.FromSeconds(1))
                .TakeWhile(_ => timerRunning.Value)
                .Repeat()
                .Select(sec => TimeSpan.FromSeconds(sec))
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);

            StartCommand = timerRunning.Inverse()
                .ToReactiveCommand()
                .WithSubscribe(() => timerRunning.TurnOn(), CompositeDisposable.Add);

            StopCommand = timerRunning
                .ToReactiveCommand()
                .WithSubscribe(() => timerRunning.TurnOff(), CompositeDisposable.Add);

            // ◆BooleanNotifierの状態が CanExecute に反映されないので、1回 On->Off しとく。
            timerRunning.SwitchValue(); timerRunning.SwitchValue();
#endif
        }
    }
}
