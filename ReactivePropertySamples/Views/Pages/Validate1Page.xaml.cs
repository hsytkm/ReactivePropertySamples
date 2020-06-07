using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class Validate1Page : Controls.MyPageControl
    {
        public Validate1Page()
        {
            InitializeComponent();
            DataContext = new TwoWay1ViewModel();
        }
    }

    class Validate1ViewModel : MyDisposableBindableBase
    {
    }
}
