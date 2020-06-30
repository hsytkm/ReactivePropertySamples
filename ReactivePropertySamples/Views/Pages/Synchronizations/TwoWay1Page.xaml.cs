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
                .ToReactivePropertyAsSynchronized(x => x.UserSettingFlag)
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
