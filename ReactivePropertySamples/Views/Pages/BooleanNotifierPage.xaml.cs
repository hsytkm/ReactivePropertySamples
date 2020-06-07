using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class BooleanNotifierPage : Controls.MyPageControl
    {
        public BooleanNotifierPage()
        {
            InitializeComponent();
            DataContext = new BooleanNotifierViewModel();
        }
    }

    class BooleanNotifierViewModel : MyDisposableBindableBase
    {
    }
}
