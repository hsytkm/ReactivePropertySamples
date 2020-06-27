using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ObservableCollectionScanPage : Controls.MyPageControl
    {
        public ObservableCollectionScanPage()
        {
            InitializeComponent();
            DataContext = new ObservableCollectionScanViewModel();
        }
    }

    class ObservableCollectionScanViewModel : MyDisposableBindableBase
    {
        public ObservableCollection<ObservableCollectionScanModel> ItemsSource { get; } =
            new ObservableCollection<ObservableCollectionScanModel>(
                Enumerable.Range(1, 5).Select(x => new ObservableCollectionScanModel($"Data {x}")));

        public ReadOnlyReactiveProperty<bool> IsUpdated { get; }

        public ObservableCollectionScanViewModel()
        {
            // [C#][ReactiveProperty] ObservableCollectionの要素の集計をして、ReactivePropertyに変換する
            //   https://qiita.com/koara-local/items/2c0c27c40339ba2b09a4?utm_source=stock_summary_mail&utm_medium=email&utm_term=hsytkm&utm_content=%5bC#][ReactiveProperty]%20ObservableCollection%E3%81%AE%E8%A6%81%E7%B4%A0%E3%81%AE%E9%9B%86%E8%A8%88%E3%82%92%E3%81%97%E3%81%A6%E3%80%81ReactiveProperty%E3%81%AB%E5%A4%89%E6%8F%9B%E3%81%99%E3%82%8B&utm_campaign=stock_summary_mail_2020-06-27
            //   Scan(初期値, (前回値, 次の値) => { return 計算結果; }) で集計ができるっぽい
            //   Aggregate は集計し終わった値だけ返しますが、Scan 集計途中の値を全て報告してくれる
            IsUpdated = ItemsSource
                .ObserveElementProperty(x => x.IsUpdated)
                .Scan(0, (counter, x) => counter + (x.Value ? 1 : (0 < counter ? -1 : 0)))
                .Select(counter => 0 < counter)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
        }
    }

    class ObservableCollectionScanModel : MyBindableBase
    {
        public string Name { get; }
        public bool IsUpdated
        {
            get => _isUpdated;
            set => SetProperty(ref _isUpdated, value);
        }
        private bool _isUpdated;
        public ObservableCollectionScanModel(string name) => Name = name;
    }
}
