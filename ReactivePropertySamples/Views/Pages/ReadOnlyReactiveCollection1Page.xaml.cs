using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ReadOnlyReactiveCollection1Page : Controls.MyPageControl
    {
        public ReadOnlyReactiveCollection1Page()
        {
            InitializeComponent();
            DataContext = new ReadOnlyReactiveCollection1ViewModel();
        }
    }

    class ReadOnlyReactiveCollection1ViewModel : MyDisposableBindableBase
    {
    }
}
