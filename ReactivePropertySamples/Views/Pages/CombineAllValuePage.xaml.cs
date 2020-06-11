using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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
        public ReactiveProperty<bool> Check1 { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> Check2 { get; } = new ReactiveProperty<bool>(initialValue: true);
        public ReactiveProperty<bool> Check3 { get; } = new ReactiveProperty<bool>(initialValue: false);
        public ReadOnlyReactiveProperty<bool> CheckAllTrue { get; }
        public ReadOnlyReactiveProperty<bool> CheckAllFalse { get; }
        public ReadOnlyReactiveProperty<bool> CheckOr { get; }
        public ReadOnlyReactiveProperty<bool> CheckOdd { get; }

        public CombineAllValueViewModel()
        {
            CheckAllTrue = new[] { Check1, Check2, Check3 }
                .CombineLatestValuesAreAllTrue()
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            CheckAllFalse = new[] { Check1, Check2, Check3 }
                .CombineLatestValuesAreAllFalse()
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            CheckOr = new[] { Check1, Check2, Check3 }
                .CombineLatest()
                .Select(flags => flags.Any(x => x))
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            CheckOdd = new[] { Check1, Check2, Check3 }
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
