using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class CountNotifierPage : Controls.MyPageControl
    {
        public CountNotifierPage()
        {
            InitializeComponent();
            DataContext = new CountNotifierViewModel();
        }
    }

    class CountNotifierViewModel : MyDisposableBindableBase
    {
    }
}
