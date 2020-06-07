using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ObserveElementPropertyChanged1Page : Controls.MyPageControl
    {
        public ObserveElementPropertyChanged1Page()
        {
            InitializeComponent();
            DataContext = new ObserveElementPropertyChanged1ViewModel();
        }
    }

    class ObserveElementPropertyChanged1ViewModel : MyDisposableBindableBase
    {
    }
}
