using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ReactiveCommand1Page : Controls.MyPageControl
    {
        public ReactiveCommand1Page()
        {
            InitializeComponent();
            DataContext = new ReactiveCommand1ViewModel();
        }
    }

    class ReactiveCommand1ViewModel : MyDisposableBindableBase
    {
        public ReactiveCommand Command1 { get; } = new ReactiveCommand();
        public ReactiveProperty<int> Counter1 { get; } = new ReactiveProperty<int>();

        public ReactiveCommand<int> Command2 { get; } = new ReactiveCommand<int>();
        public ReactiveProperty<int> Counter2 { get; } = new ReactiveProperty<int>();

        public ReactiveCommand Command31 { get; } // ToReactiveCommand() で生成するので初期化不要
        public ReactiveProperty<bool> CheckFlag31 { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<int> Counter3 { get; } = new ReactiveProperty<int>();

        public ReactiveCommand Command32 { get; } // ToReactiveCommand() で生成するので初期化不要

        public ReactiveCommand1ViewModel()
        {
            Command1
                .Subscribe(() => ++Counter1.Value)
                .AddTo(CompositeDisposable);

            Command2
                .Subscribe(x => Counter2.Value += x)
                .AddTo(CompositeDisposable);

            // IObservable<bool> から ReactiveCommand を作成
            // View の CheckBox により、CanExecute  を切り替え
            Command31 = CheckFlag31.ToReactiveCommand()
                .WithSubscribe(() => ++Counter3.Value, CompositeDisposable.Add);

            var updateTimeTrigger = new Subject<Unit>();
            CompositeDisposable.Add(updateTimeTrigger);

            // IObservable<bool> から ReactiveCommand を作成
            // 実行後に一定時間は CanExecute を無効にする (ひねくれずに AsyncReactiveCommand を使えばよいと思う)
            Command32 = Observable.Merge(
                    updateTimeTrigger.Select(_ => false),
                    updateTimeTrigger.Delay(TimeSpan.FromSeconds(0.5)).Select(_ => true))
                .ToReactiveCommand()
                .AddTo(CompositeDisposable);

            Command32
                .Do(_ => updateTimeTrigger.OnNext(Unit.Default))
                .Subscribe(_ => ++Counter3.Value)
                .AddTo(CompositeDisposable);
        }
    }
}
