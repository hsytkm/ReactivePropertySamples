#nullable disable
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class CombineAllValuePage : Controls.MyPageControl
    {
        public CombineAllValuePage()
        {
            InitializeComponent();
            DataContext = new CombineAllValueViewModel();
        }
    }

    class CombineAllValueViewModel : MyDisposableBindableBase
    {
        public IReactiveProperty<bool> Check1 { get; } = new ReactiveProperty<bool>();
        public IReactiveProperty<bool> Check2 { get; } = new ReactiveProperty<bool>(initialValue: true);
        public BooleanNotifier Check3 { get; } = new BooleanNotifier(initialValue: false);
        public IReadOnlyReactiveProperty<bool> CheckAllTrue { get; }
        public IReadOnlyReactiveProperty<bool> CheckAllFalse { get; }
        public IReadOnlyReactiveProperty<bool> CheckOr { get; }
        public IReadOnlyReactiveProperty<bool> CheckOdd { get; }

        public CombineAllValueViewModel()
        {
            CheckAllTrue = new IObservable<bool>[] { Check1, Check2, Check3 }
                .CombineLatestValuesAreAllTrue()
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            CheckAllFalse = new IObservable<bool>[] { Check1, Check2, Check3 }
                .CombineLatestValuesAreAllFalse()
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            CheckOr = new IObservable<bool>[] { Check1, Check2, Check3 }
                .CombineLatest()
                .Select(flags => flags.Any(x => x))
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            CheckOdd = new IObservable<bool>[] { Check1, Check2, Check3.Inverse().Inverse() }   // NotNotなので意味なし
                .CombineLatest()
                .Select(flags =>
                {
                    var count = 0;
                    foreach (var flag in flags)
                    {
                        if (flag) ++count;
                    }
                    return ((count & 1) == 1);
                })
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
        }
    }
}
