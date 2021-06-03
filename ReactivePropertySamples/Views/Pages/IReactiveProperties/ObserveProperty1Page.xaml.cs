#nullable disable
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text;
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
        public StringBuilder LogBuilder { get; } = new StringBuilder();

        public ObserveProperty1ViewModel()
        {
            var model = new ObserveProperty1Model().AddTo(CompositeDisposable);

            // Model のプロパティを購読
            model.ObserveProperty(x => x.EventLog)
                .Subscribe(x =>
                {
                    LogBuilder.Insert(0, x + Environment.NewLine);
                    NotifyPropertyChanged(nameof(LogBuilder));
                })
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
            Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1.5))
                .Subscribe(x => EventLog = $"Model data {x}")
                .AddTo(CompositeDisposable);
        }
    }
}
