using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class TwoWay1Page : Controls.MyPageControl
    {
        public TwoWay1Page()
        {
            InitializeComponent();
            DataContext = new TwoWay1ViewModel();
        }
    }

    class TwoWay1ViewModel : MyDisposableBindableBase
    {
        public IReactiveProperty<bool> IsFunctionEnable { get; }

        public TwoWay1ViewModel()
        {
            var model = new TwoWay1Model();

            IsFunctionEnable = model
#if false
                .ToReactivePropertyAsSynchronized(x => x.UserSettingFlag)
#else
                // ReactiveProperty 7.4.0 以上
                // RpSlimなので Validation は使えない。割と使いたいかも…
                .ToReactivePropertySlimAsSynchronized(x => x.UserSettingFlag)
#endif
                .AddTo(CompositeDisposable);
        }
    }

    class TwoWay1Model : MyBindableBase
    {
        public bool UserSettingFlag
        {
            get => _userSettingFlag;
            set => SetProperty(ref _userSettingFlag, value);
        }
        private bool _userSettingFlag;
    }
}
