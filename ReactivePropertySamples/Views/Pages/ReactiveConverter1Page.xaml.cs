using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ReactiveConverter1Page : Controls.MyPageControl
    {
        public ReactiveConverter1Page()
        {
            InitializeComponent();
            DataContext = new ReactiveConverter1ViewModel();
        }
    }

    class ReactiveConverter1ViewModel : MyDisposableBindableBase
    {
        public ReactiveProperty<Point> MouseMovePoint { get; } = new ReactiveProperty<Point>();
    }
}
