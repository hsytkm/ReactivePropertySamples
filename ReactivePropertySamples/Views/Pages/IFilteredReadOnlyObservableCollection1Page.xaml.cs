using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class IFilteredReadOnlyObservableCollection1Page : Controls.MyPageControl
    {
        public IFilteredReadOnlyObservableCollection1Page()
        {
            InitializeComponent();
            DataContext = new IFilteredReadOnlyObservableCollection1ViewModel();
        }
    }

    class ValueHolder : MyDisposableBindableBase
    {
        public int Id { get; }
        public int Value
        {
            get => _value;
            private set => SetProperty(ref _value, value);
        }
        private int _value;

        public ValueHolder(int id)
        {
            Id = id;

            Observable.Interval(TimeSpan.FromSeconds(1))
                .ObserveOnUIDispatcher()
                .Subscribe(_ => ++Value)
                .AddTo(CompositeDisposable);
        }
    }

    class IFilteredReadOnlyObservableCollection1ViewModel : MyDisposableBindableBase
    {
        public ReactiveCommand AddCommand { get; }
        public ReactiveCollection<ValueHolder> SourceValues { get; }
        public IFilteredReadOnlyObservableCollection<ValueHolder> FilteredValues { get; }

        public IFilteredReadOnlyObservableCollection1ViewModel()
        {
            AddCommand = new ReactiveCommand()
                .AddTo(CompositeDisposable);

            SourceValues = AddCommand
                .Select(_ => new ValueHolder(SourceValues.Count))
                .ToReactiveCollection()
                .AddTo(CompositeDisposable);

            FilteredValues = SourceValues
                .ToFilteredReadOnlyObservableCollection(x => (x.Value % 3) == 0)
                .AddTo(CompositeDisposable);
        }
    }
}
