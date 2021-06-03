#nullable disable
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ObserveElementPropertyChanged1Page : Controls.MyPageControl
    {
        public ObserveElementPropertyChanged1Page()
        {
            InitializeComponent();
            DataContext = new ObserveElementPropertyChanged1ViewModel();
        }
    }

    class ObserveElementPropertyChanged1ViewModel : MyDisposableBindableBase
    {
        public ObservableCollection<DQPlayer> Players { get; } = DQPlayer.Party.ToObservableCollection();

        // ◆StringBuilder を使ったけど、Rp でログを TextBox に表示するには、どう実装するのだろうか…
        public StringBuilder PlayersStatusLog { get; } = new StringBuilder();

        public ObserveElementPropertyChanged1ViewModel()
        {
            // 各プロパティの変更を監視する（知れるのは インスタンス と 変化プロパティ で、変更後の値 は取れなさげ）
            Players.ObserveElementPropertyChanged()
                .Select(x => $"{x.Sender.Name} は {x.EventArgs.PropertyName} が上がった。")
                .Subscribe(msg =>
                {
                    PlayersStatusLog.Insert(0, msg + Environment.NewLine);  // 先頭に追加
                    NotifyPropertyChanged(nameof(PlayersStatusLog));
                })
                .AddTo(CompositeDisposable);

        }
    }
}
