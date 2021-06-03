#nullable disable
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class EventToReactiveProperty3Page : Controls.MyPageControl
    {
        public EventToReactiveProperty3Page()
        {
            InitializeComponent();
            DataContext = new EventToReactiveProperty3ViewModel();
        }
    }

    class EventToReactiveProperty3ViewModel : MyDisposableBindableBase
    {
        public BooleanNotifier MouseLongPushing1 { get; } = new BooleanNotifier(false);
        public IReactiveProperty<bool> MouseLongPushing2 { get; }
        //public IReadOnlyReactiveProperty<bool> MouseLongPushing3 { get; }

        public IReactiveProperty<Unit> MouseDownUnit { get; } =
            new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);
        public IReactiveProperty<Unit> MouseUpUnit { get; } =
            new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);

        public EventToReactiveProperty3ViewModel()
        {
            // Throttleメソッドは指定した間、新たな値が発行されなかったら最後に発行された値を後続に流す
            var pushingSpanMsec = 300.0;

            /*
             *  一番最初に思い付いた実装。ダサい
             */
            MouseDownUnit
                .Throttle(TimeSpan.FromMilliseconds(pushingSpanMsec))
                .TakeUntil(MouseUpUnit)                         // ちょん押しを弾く
                .ObserveOnUIDispatcher()
                .Repeat()
                .Subscribe(_ => MouseLongPushing1.TurnOn())
                .AddTo(CompositeDisposable);

            MouseUpUnit
                .Where(_ => MouseLongPushing1.Value)             // Push中のみ解除判定
                .Subscribe(_ => MouseLongPushing1.TurnOff())
                .AddTo(CompositeDisposable);

            /*
             *  Finally を使って、上の処理を一文に無理やりまとめた版。
             *  ReadOnlyReactiveProperty に出来ていないのが残念…
             */
            MouseLongPushing2 = MouseDownUnit
                .Throttle(TimeSpan.FromMilliseconds(pushingSpanMsec))
                .TakeUntil(MouseUpUnit)                         // ちょん押しを弾く
                .ObserveOnUIDispatcher()
                .Finally(() => MouseLongPushing2.Value = false)
                .Repeat()
                .Select(_ => true)
                .ToReactiveProperty()
                .AddTo(CompositeDisposable);

            /*
             *  ◆ReadOnlyReactiveProperty を使って、一文にまとめられへんかなぁ…
             */

        }
    }
}
