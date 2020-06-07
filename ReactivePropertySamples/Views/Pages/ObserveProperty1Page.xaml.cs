using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ObserveProperty1Page : Controls.MyPageControl
    {
        public ObserveProperty1Page()
        {
            InitializeComponent();
            DataContext = new ObserveProperty1ViewModel();
        }
    }

    class ObserveProperty1ViewModel : MyDisposableBindableBase
    {
        private readonly ObserveProperty1Model _model;

        public ReadOnlyReactiveProperty<string> LatestLog { get; }

        public ReadOnlyReactiveCollection<string> LatestLogs { get; }
        //public ObservableCollection<string> LatestLogs1 { get; } = new ObservableCollection<string>();
        //public ReactiveCollection<string> LatestLogCollection2 { get; } = new ReactiveCollection<string>();

        public ObserveProperty1ViewModel()
        {
            _model = new ObserveProperty1Model();
            CompositeDisposable.Add(_model);

            // Model のプロパティを購読
            LatestLog = _model
                .ObserveProperty(x => x.EventLog)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            var logs = new ObservableCollection<string>();

            // 購読したデータをコレクション先頭に追加
            LatestLog
                .Subscribe(x => logs.Insert(0, x))
                .AddTo(CompositeDisposable);

            LatestLogs = logs
                .ToReadOnlyReactiveCollection()
                .AddTo(CompositeDisposable);

        }
    }

    class ObserveProperty1Model : MyDisposableBindableBase
    {
        public string EventLog
        {
            get => _eventLog;
            private set => SetProperty(ref _eventLog, value);
        }
        private string _eventLog;

        public ObserveProperty1Model()
        {
            // 1秒後から1.5秒間隔のタイマー
            var timer = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1.5));

            var disposable = timer
                .Subscribe(x => EventLog = $"message {x}")
                .AddTo(CompositeDisposable);
        }
    }
}
