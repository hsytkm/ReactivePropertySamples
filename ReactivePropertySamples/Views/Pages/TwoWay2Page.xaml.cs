using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class TwoWay2Page : Controls.MyPageControl
    {
        public TwoWay2Page()
        {
            InitializeComponent();
            DataContext = new TwoWay2ViewModel();
        }
    }

    class TwoWay2ViewModel : MyDisposableBindableBase
    {
        private readonly TwoWay2Model _model = new TwoWay2Model();


        public TwoWay2ViewModel()
        {
        }
    }

    class TwoWay2Model : MyBindableBase
    {
    }
}
