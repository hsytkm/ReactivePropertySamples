using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

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
    }
}
