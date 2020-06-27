using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input.Manipulations;
using System.Windows.Threading;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ScheduledNotifierPage : Controls.MyPageControl
    {
        public ScheduledNotifierPage()
        {
            InitializeComponent();
            DataContext = new ScheduledNotifierViewModel();
        }
    }

    class ScheduledNotifierViewModel : MyDisposableBindableBase
    {
        public ScheduledNotifier<double> ScheduledNotifier { get; } = new ScheduledNotifier<double>();

        public AsyncReactiveCommand HeavyProcessCommand { get; }

        public ReadOnlyReactiveProperty<double> ProcessingProgress { get; }

        public ScheduledNotifierViewModel()
        {
            ProcessingProgress = ScheduledNotifier
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            HeavyProcessCommand = new AsyncReactiveCommand()
                .AddTo(CompositeDisposable);

            HeavyProcessCommand
                .Subscribe(async () => await HeavyProcessAsync(ScheduledNotifier))
                .AddTo(CompositeDisposable);

#if false
            var scheduledNotifier = new ScheduledNotifier<int>();
            var progress = scheduledNotifier as IProgress<int>;

            scheduledNotifier.Report(10);
            scheduledNotifier.Report(100, TimeSpan.FromSeconds(3));
            progress.Report(22);
#endif
        }

        // 進捗報告ありの重い処理
        // ScheduledNotifier<T> は IProgress<T> を継承している
        private static async Task HeavyProcessAsync(IProgress<double> progress)
        {
            var max = 6;
            for (int i = 0; i < max; ++i)
            {
                progress?.Report((double)i / max);
                await Task.Delay(1000);
            }
            progress?.Report(1);    //100%
        }

    }
}
