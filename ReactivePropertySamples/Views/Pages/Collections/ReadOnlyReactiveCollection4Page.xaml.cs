using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Extensions;
using ReactivePropertySamples.Infrastructures;
using ReactivePropertySamples.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages;

public partial class ReadOnlyReactiveCollection4Page : Controls.MyPageControl
{
    public ReadOnlyReactiveCollection4Page()
    {
        DataContext = new ReadOnlyReactiveCollection4ViewModel();
        InitializeComponent();
    }
}

class ReadOnlyReactiveCollection4ViewModel : MyDisposableBindableBase
{
    public ReadOnlyReactiveCollection<IdRandomValuePair> Items { get; }
    public MyCommand<IdRandomValuePair> UpdateValueCommand { get; } = new(x => x?.UpdateValue());

    public ReadOnlyReactiveCollection4ViewModel()
    {
        Items = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(2))
            .Select(x => new IdRandomValuePair(x))
            .ToReadOnlyReactiveCollection()
            .AddTo(CompositeDisposable);
    }
}

class IdRandomValuePair : MyBindableBase
{
    static readonly Random _random = new Random(1214);

    public long Id { get; }

    public int Value
    {
        get => _value;
        private set => SetProperty(ref _value, value);
    }
    int _value = int.MinValue;

    public IdRandomValuePair(long id)
    {
        Id = id;
        UpdateValue();
    }

    public void UpdateValue()
    {
        int newValue = _random.Next(0, 100);
        //if (Value > 0) System.Diagnostics.Debug.WriteLine($"{nameof(Id)}={Id}, {nameof(Value)}={Value} -> {newValue}");
        Value = newValue;
    }

    public override string ToString() => $"{nameof(Id)}={Id}, {nameof(Value)}={Value}";
}
