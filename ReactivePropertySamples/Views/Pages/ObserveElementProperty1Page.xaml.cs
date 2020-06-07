using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ObserveElementProperty1Page : Controls.MyPageControl
    {
        public ObserveElementProperty1Page()
        {
            InitializeComponent();
            DataContext = new ObserveElementProperty1ViewModel();
        }
    }

    class ObserveElementProperty1ViewModel : MyDisposableBindableBase
    {
    }
}
