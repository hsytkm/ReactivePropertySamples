using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class BooleanNotifier2Page : Controls.MyPageControl
    {
        public BooleanNotifier2Page()
        {
            InitializeComponent();
            DataContext = new BooleanNotifier2ViewModel();
        }
    }

    // https://okazuki.jp/ReactiveProperty/features/Notifiers.html#booleannotifier
    class BooleanNotifier2ViewModel : MyDisposableBindableBase
    {
        public ReactiveCommand BooleanNotifierButtonCommand0 { get; }
        public ReactiveCommand ReactivePropertyButtonCommand { get; }
        public ReactiveCommand BooleanNotifierButtonCommand1 { get; }
        public ReactiveCommand BooleanNotifierButtonCommand2 { get; }

        public BooleanNotifier2ViewModel()
        {
            var booleanNotifier = new BooleanNotifier(initialValue: false);
            var reactiveProperty = new ReactiveProperty<bool>(initialValue: false).AddTo(CompositeDisposable);

            BooleanNotifierButtonCommand0 = booleanNotifier.ToReactiveCommand().AddTo(CompositeDisposable);
            ReactivePropertyButtonCommand = reactiveProperty.ToReactiveCommand().AddTo(CompositeDisposable);

            // CanExecute method of ReactiveCommand returns true as default.
            // So, you set initialValue explicitly to `booleanNotifier.Value`.
            BooleanNotifierButtonCommand1 = booleanNotifier
                .ToReactiveCommand(initialValue: booleanNotifier.Value)
                .AddTo(CompositeDisposable);

            // Or if you would like to convert to something using Select and others
            // before calling ToReactiveCommand, you can use StartWith.
            BooleanNotifierButtonCommand2 = booleanNotifier
                .StartWith(booleanNotifier.Value)
                .Select(x => Something(x))
                .ToReactiveCommand()
                .AddTo(CompositeDisposable);

            static bool Something(bool b) => b;
        }
    }
}
