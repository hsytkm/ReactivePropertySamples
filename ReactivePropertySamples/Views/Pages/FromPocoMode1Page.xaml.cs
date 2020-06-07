using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class FromPocoMode1Page : Controls.MyPageControl
    {
        public FromPocoMode1Page()
        {
            InitializeComponent();
            DataContext = new FromPocoMode1ViewModel();
        }
    }

    class FromPocoMode1ViewModel : MyDisposableBindableBase
    {
    }
}
