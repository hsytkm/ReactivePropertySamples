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
        public IReactiveProperty<Unit> MouseUpUnit { get; } =
            new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);

        public ReactiveConverter4ViewModel()
        {
            // ◆ダブルクリック実装中…

            // ダブルクリックイベントの自作  http://y-maeyama.hatenablog.com/entry/20110313/1300002095
            var preDoubleClick = MouseDownUnit
                //.Do(x => Debug.WriteLine($"SingleClick: {x.Time} {x.Point.X} x {x.Point.Y}"))
                .Select(_ => DateTime.Now)
                .Pairwise()
                .Select(pair => pair.NewItem.Subtract(pair.OldItem))
                .Where(span => span <= TimeSpan.FromMilliseconds(500))
                .Select(_ => DateTime.Now)
                .ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe)
                .AddTo(CompositeDisposable);

            // ダブルクリックの2回が、3クリック目で2度目のダブルクリックになる対策
            // (ダブルクリック後に一定時間が経過するまでダブルクリックを採用しない)
            preDoubleClick
                .Pairwise()
                .Select(pair => pair.NewItem.Subtract(pair.OldItem))
                .Where(span => span >= TimeSpan.FromMilliseconds(200))
                .ToUnit()
                .Subscribe(_ => MouseDoubleClickCounter.Increment())
                .AddTo(CompositeDisposable);
        }
    }
}
