using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

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

        public ReactiveCommand Command3 { get; } // ToReactiveCommand() で生成するので初期化不要
        public ReactiveProperty<bool> CheckFlag3 { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<int> Counter3 { get; } = new ReactiveProperty<int>();

        public ReactiveCommand1ViewModel()
        {
            Command1
                .Subscribe(() => ++Counter1.Value)
                .AddTo(CompositeDisposable);

            Command2
                .Subscribe(x => Counter2.Value += x)
                .AddTo(CompositeDisposable);

            // IObservable<bool> から ReactiveCommand を作成
            Command3 = CheckFlag3
                .ToReactiveCommand()
                .AddTo(CompositeDisposable);
            Command3
                .Subscribe(() => ++Counter3.Value)
                .AddTo(CompositeDisposable);

        }
    }
}
