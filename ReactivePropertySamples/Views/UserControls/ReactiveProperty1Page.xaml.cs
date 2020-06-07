using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.UserControls
{
    public partial class ReactiveProperty1Page : Controls.MyPageControl
    {
        public ReactiveProperty1Page()
        {
            InitializeComponent();
            DataContext = new ReactiveProperty1ViewModel();
        }
    }

    class ReactiveProperty1ViewModel : MyDisposableBindableBase
    {
        // ReactivePropertyMode.None だと、初回の RaiseLatestValueOnSubscribe の ToUpper() で
        // nullアクセスになるので、RaiseLatestValueOnSubscribe の設定を除去する。
        // x?.ToUpper() にすれば済むんやけど、勉強サンプルやからね。
        public ReactiveProperty<string> InputText { get; } =
            new ReactiveProperty<string>(mode: ReactivePropertyMode.DistinctUntilChanged);

        public ReadOnlyReactiveProperty<string> OutputText { get; }

        public ReactiveProperty1ViewModel()
        {
            OutputText = InputText.Select(x => x.ToUpper())
                .Delay(TimeSpan.FromSeconds(1))
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
        }
    }
}
