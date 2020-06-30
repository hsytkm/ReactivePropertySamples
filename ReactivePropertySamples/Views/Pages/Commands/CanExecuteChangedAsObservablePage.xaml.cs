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
using System.Windows.Input;
using System.Windows.Threading;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class CanExecuteChangedAsObservablePage : Controls.MyPageControl
    {
        public CanExecuteChangedAsObservablePage()
        {
            InitializeComponent();
            DataContext = new CanExecuteChangedAsObservableViewModel();
        }
    }

    class CanExecuteChangedAsObservableViewModel : MyDisposableBindableBase
    {
        public ReactiveProperty<bool> IsChecked1 { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsChecked2 { get; } = new ReactiveProperty<bool>();
        public ReactiveCommand IncrementCommand { get; }
        public IReadOnlyReactiveProperty<bool> CanExecuteIncrementCommand { get; }
        public CountNotifier Counter { get; } = new CountNotifier();

        public CanExecuteChangedAsObservableViewModel()
        {
            IncrementCommand = new[] { IsChecked1, IsChecked2 }
                .CombineLatestValuesAreAllTrue()
                .ToReactiveCommand()
                .WithSubscribe(() => Counter.Increment(), CompositeDisposable.Add);

            CanExecuteIncrementCommand = IncrementCommand.CanExecuteChangedAsObservable()
                .Select(_ => IncrementCommand.CanExecute())
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
        }
    }
}
