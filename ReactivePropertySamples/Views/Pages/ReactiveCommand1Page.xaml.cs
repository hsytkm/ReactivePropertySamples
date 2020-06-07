using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ReactiveCommand1Page : Controls.MyPageControl
    {
        public ReactiveCommand1Page()
        {
            InitializeComponent();
            DataContext = new ReactiveCommand1ViewModel();
        }
    }

    class ReactiveCommand1ViewModel : MyDisposableBindableBase
    {
    }
}
