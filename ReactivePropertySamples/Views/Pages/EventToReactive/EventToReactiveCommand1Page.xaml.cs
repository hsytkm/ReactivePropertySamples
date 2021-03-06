﻿using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class EventToReactiveCommand1Page : Controls.MyPageControl
    {
        public EventToReactiveCommand1Page()
        {
            InitializeComponent();
            DataContext = new EventToReactiveCommand1ViewModel();
        }
    }

    class EventToReactiveCommand1ViewModel : MyDisposableBindableBase
    {
        public IReactiveProperty<Point> MouseMovePoint1 { get; } = new ReactiveProperty<Point>();
        public ReactiveCommand<Point> MouseMoveCommandFromViewEvents { get; } = new ReactiveCommand<Point>();
        public IReactiveProperty<string> MouseMoveMessage { get; } = new ReactiveProperty<string>();

        public EventToReactiveCommand1ViewModel()
        {
            MouseMoveCommandFromViewEvents
                .Subscribe(p => MouseMoveMessage.Value = $"EventToReactiveCommand: {p:f2}")
                .AddTo(CompositeDisposable);
        }

    }
}
