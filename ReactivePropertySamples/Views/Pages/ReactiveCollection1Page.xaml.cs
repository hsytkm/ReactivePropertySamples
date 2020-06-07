using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ReactiveCollection1Page : Controls.MyPageControl
    {
        public ReactiveCollection1Page()
        {
            InitializeComponent();
            DataContext = new ReactiveCollection1ViewModel();
        }
    }

    class ReactiveCollection1ViewModel : MyDisposableBindableBase
    {
    }
}
