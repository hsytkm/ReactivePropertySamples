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
using System.Windows.Input;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ReactiveConverter4Page : Controls.MyPageControl
    {
        public ReactiveConverter4Page()
        {
            InitializeComponent();

            UpdateTextBlock(0);
            DataContext = new ReactiveConverter4ViewModel();
        }

        private int _counter;
        private void UpdateTextBlock(int count) => doubleClickTextBlock.Text = $"Control.MouseDoubleClick = {count}";
        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e) => UpdateTextBlock(++_counter);
    }

    class ReactiveConverter4ViewModel : MyDisposableBindableBase
    {
        public CountNotifier MouseDoubleClickCounter { get; } = new CountNotifier();

        public IReactiveProperty<Unit> MouseDownUnit { get; } =
            new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);

        public ReactiveConverter4ViewModel()
        {
            // Reactive Framework / DoubleClick https://stackoverflow.com/questions/5228364/reactive-framework-doubleclick

            // ダブクリの判定にローカル変数を使ってるのがイマイチ…
            var eventAcceptedTime = DateTime.Now;

            MouseDownUnit
                .TimeInterval()
                .Skip(1)
                .Where(ti =>
                {
                    // 前回の MouseDown から一定時間が経過していればダブクリと言わない
                    if (ti.Interval > TimeSpan.FromMilliseconds(500))
                        return false;

                    var now = DateTime.Now;

                    // 前回のダブクリ受付から一定時間が経過するまでは、次のダブクリを受け付けない
                    if (now - eventAcceptedTime < TimeSpan.FromMilliseconds(500))
                        return false;

                    // ダブクリ受付時間の更新
                    eventAcceptedTime = now;
                    return true;
                })
                .Subscribe(_ => MouseDoubleClickCounter.Increment())
                .AddTo(CompositeDisposable);
        }
    }
}
