using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class BooleanNotifier1Page : Controls.MyPageControl
    {
        public BooleanNotifier1Page()
        {
            InitializeComponent();
            DataContext = new BooleanNotifier1ViewModel();
        }
    }

    class BooleanNotifier1ViewModel : MyDisposableBindableBase
    {
        public IReactiveProperty<double> InputValue { get; }
            = new ReactiveProperty<double>(initialValue: 6);

        // 2の倍数チェック
        public BooleanNotifier BoolNotifierMultiple2 { get; } = new BooleanNotifier();

        // 3の倍数チェック
        public BooleanNotifier BoolNotifierMultiple3 { get; } = new BooleanNotifier();

        public BooleanNotifier1ViewModel()
        {
            InputValue
                .Subscribe(x =>
                {
                    // 2の倍数チェック(Valueで操作)  ※どちらでも同じ
                    BoolNotifierMultiple2.Value = (x % 2 == 0);

                    // 3の倍数チェック(メソッドで操作)  ※どちらでも同じ
                    if (x % 3 == 0)
                    {
                        BoolNotifierMultiple3.TurnOn();
                    }
                    else
                    {
                        BoolNotifierMultiple3.TurnOff();
                    }

                    // 以下で値の切り替えができるよ
                    //boolNotifier.SwitchValue();
                })
                .AddTo(CompositeDisposable);
        }
    }
}
