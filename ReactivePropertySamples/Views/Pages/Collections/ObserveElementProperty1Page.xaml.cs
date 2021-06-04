using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ObserveElementProperty1Page : Controls.MyPageControl
    {
        public ObserveElementProperty1Page()
        {
            InitializeComponent();
            DataContext = new ObserveElementProperty1ViewModel();
        }
    }

    class DQPlayer : MyBindableBase
    {
        private static readonly Random _random = new Random();
        public string Name { get; }
        public int Lv
        {
            get => _lv;
            private set => SetProperty(ref _lv, value);
        }
        private int _lv;
        public int HP
        {
            get => _hp;
            private set => SetProperty(ref _hp, value);
        }
        private int _hp;
        public int MP
        {
            get => _mp;
            private set => SetProperty(ref _mp, value);
        }
        private int _mp;

        public DQPlayer(string name, int hp, int mp) =>
            (Name, Lv, HP, MP) = (name, 1, hp, mp);

        public ICommand LvUpCommand => _lvUpCommand ??=
            new MyCommand(() =>
            {
                HP += _random.Next(1, 10);
                MP += _random.Next(0, 5);

                // LVの変化時に情報を表示するので、LVは最後に更新すること
                Lv++;
            });
        private ICommand _lvUpCommand = default!;

        public static IEnumerable<DQPlayer> Party { get; } =
            new[]
            {
                new DQPlayer("勇者", 15, 5),
                new DQPlayer("戦士", 25, 0),
                new DQPlayer("僧侶", 10, 10),
                new DQPlayer("魔法", 5, 15),
            };
    }

    class ObserveElementProperty1ViewModel : MyDisposableBindableBase
    {
        public ObservableCollection<DQPlayer> Players { get; } = DQPlayer.Party.ToObservableCollection();

        // ◆StringBuilder を使ったけど、Rp でログを TextBox に表示するには、どう実装するのだろうか…
        public StringBuilder PlayersStatusLog { get; } = new StringBuilder();

        public ObserveElementProperty1ViewModel()
        {
            // 特定のプロパティの変更を監視する (LV変化時に HP/MP も表示)
            Players.ObserveElementProperty(x => x.Lv)
                .Where(x => x.Value > 1)
                .Select(x => $"{x.Instance.Name} は {x.Property.Name} が {x.Value} に上がった。(HP={x.Instance.HP}, MP={x.Instance.MP})")
                .Subscribe(msg =>
                {
                    PlayersStatusLog.Insert(0, msg + Environment.NewLine);  // 先頭に追加
                    NotifyPropertyChanged(nameof(PlayersStatusLog));
                })
                .AddTo(CompositeDisposable);

        }
    }
}
