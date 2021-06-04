using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace ReactivePropertySamples.Views.Pages
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
        // Skip(1) でも解消するけど、Mode 変える方がスマートやね。
        public ReactiveProperty<string> InputText { get; } =
            new ReactiveProperty<string>(mode: ReactivePropertyMode.DistinctUntilChanged);

        // Value の変更ない場面で、強制的に PropertyChanged を発生させる
        public ICommand ForceNotifyCommand => _forceNotifyCommand ??=
            new MyCommand(() => InputText.ForceNotify());
        private ICommand _forceNotifyCommand = default!;

        public StringBuilder OutputTexts { get; } = new StringBuilder();

        public ReactiveProperty1ViewModel()
        {
            InputText
                .Select(x => x.ToUpperInvariant())
                .Delay(TimeSpan.FromSeconds(1))
                .Select(x => $"{DateTime.Now:hh:mm:ss.ff} : {x}"+ Environment.NewLine)
                .Subscribe(x =>
                {
                    OutputTexts.Insert(0, x);
                    NotifyPropertyChanged(nameof(OutputTexts));
                })
                .AddTo(CompositeDisposable);

        }
    }
}
