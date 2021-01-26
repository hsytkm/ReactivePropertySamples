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
        public IReadOnlyReactiveProperty<TimeSpan> MyTimer1 { get; }
        public IReadOnlyReactiveProperty<TimeSpan> MyTimer2 { get; }

        public TimerStartCommandViewModel()
        {
            // ctorからカウント
            var ctorDateTime = DateTime.Now;
            MyTimer1 = Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(_ => DateTime.Now - ctorDateTime)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);

            // Command.Executeからカウント  ◆もう少しかっちょよい実装ない？
            var clickDateTime = DateTime.MinValue;
            MyTimer2 = Observable.Interval(TimeSpan.FromSeconds(1))
                .Where(_ => clickDateTime != DateTime.MinValue)
                .Select(_ => DateTime.Now - clickDateTime)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);

            StartCommand = new ReactiveCommand()
                .WithSubscribe(() => clickDateTime = DateTime.Now, CompositeDisposable.Add);

        }
    }
}
