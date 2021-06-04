using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

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
        public CountNotifier Counter1 { get; } = new CountNotifier();

        public ReactiveCommand<int> Command2 { get; } // コンストラクタで生成するので初期化不要
        public CountNotifier Counter2 { get; } = new CountNotifier();

        public ICommand Command31 { get; } // コンストラクタで生成するので初期化不要
        public BooleanNotifier CheckFlag31 { get; } = new BooleanNotifier();
        public CountNotifier Counter3 { get; } = new CountNotifier();

        public ReactiveCommand Command32 { get; } // コンストラクタで生成するので初期化不要

        public ReactiveCommand1ViewModel()
        {
            // 宣言時にインスタンスを作ってて、コンストラクタで Subscribe()
            Command1
                .Subscribe(() => Counter1.Increment())
                .AddTo(CompositeDisposable);

            // View の CommandParameter を加算。 WithSubscribeにより宣言からDispose登録まで一気通貫。
            Command2 = new ReactiveCommand<int>()
                .WithSubscribe(x => Counter2.Increment(x), CompositeDisposable.Add);

            // CheckBox により（IObservable<bool>）から ReactiveCommand を作成
            Command31 = CheckFlag31.ToReactiveCommand(initialValue: false)
                .WithSubscribe(() => Counter3.Increment(), CompositeDisposable.Add);

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
                .Subscribe(_ => Counter3.Increment())
                .AddTo(CompositeDisposable);
        }
    }
}
