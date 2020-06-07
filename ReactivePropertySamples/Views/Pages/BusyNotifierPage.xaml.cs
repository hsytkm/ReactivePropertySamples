using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class BusyNotifierPage : Controls.MyPageControl
    {
        public BusyNotifierPage()
        {
            InitializeComponent();
            DataContext = new BusyNotifierViewModel();
        }
    }

    class BusyNotifierViewModel : MyDisposableBindableBase
    {
    }
}
