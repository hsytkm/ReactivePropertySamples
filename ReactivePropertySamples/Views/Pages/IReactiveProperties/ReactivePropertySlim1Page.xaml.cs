using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ReactivePropertySlim1Page : Controls.MyPageControl
    {
        public ReactivePropertySlim1Page()
        {
            InitializeComponent();
            DataContext = new ReactivePropertySlim1ViewModel();
        }
    }

    class ReactivePropertySlim1ViewModel : MyDisposableBindableBase
    {
        // ReactivePropertySlim
        //   https://github.com/runceel/ReactiveProperty/blob/8376c096d327a3c3a532c398611b547652e7f4c7/docs/docs/features/ReactivePropertySlim.md

        // ReactivePropertyMode.None だと、初回の RaiseLatestValueOnSubscribe の ToUpper() で
        // nullアクセスになるので、RaiseLatestValueOnSubscribe の設定を除去する。
        // x?.ToUpper() にすれば済むんやけど、勉強サンプルやからね。
        // Skip(1) でも解消するけど、Mode 変える方がスマートやね。
        public ReactivePropertySlim<string> InputText { get; } =
            new ReactivePropertySlim<string>(mode: ReactivePropertyMode.DistinctUntilChanged);

        // Value の変更ない場面で、強制的に PropertyChanged を発生させる
        public ICommand ForceNotifyCommand => _forceNotifyCommand ??=
            new MyCommand(() => InputText.ForceNotify());
        private ICommand _forceNotifyCommand;

        public StringBuilder OutputTexts { get; } = new StringBuilder();

        public ReactivePropertySlim1ViewModel()
        {
            InputText
                .Select(x => x.ToUpper())
                .Delay(TimeSpan.FromSeconds(1))
                .Select(x => $"{DateTime.Now:hh:mm:ss.ff} : {x}" + Environment.NewLine)
                .Subscribe(x =>
                {
                    OutputTexts.Insert(0, x);
                    NotifyPropertyChanged(nameof(OutputTexts));
                })
                .AddTo(CompositeDisposable);
        }
    }
}
