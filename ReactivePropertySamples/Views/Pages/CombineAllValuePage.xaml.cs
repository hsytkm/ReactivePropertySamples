using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class CombineAllValuePage : Controls.MyPageControl
    {
        public CombineAllValuePage()
        {
            InitializeComponent();
            DataContext = new CombineAllValueViewModel();
        }
    }

    class CombineAllValueViewModel : MyDisposableBindableBase
    {
    }
}
