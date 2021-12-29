using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Extensions;
using ReactivePropertySamples.Infrastructures;
using ReactivePropertySamples.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ReadOnlyReactiveCollection3Page : Controls.MyPageControl
    {
        public ReadOnlyReactiveCollection3Page()
        {
            DataContext = new ReadOnlyReactiveCollection3ViewModel();
            InitializeComponent();
        }
    }

    class ReadOnlyReactiveCollection3ViewModel : MyDisposableBindableBase
    {
        private readonly static ReadOnlyReactiveCollection3Model _model = new();

        public ReactiveCommand<PersonRORC> DeleteItemCommand { get; }
        public RoReactiveCollectionSelectedItemPair<PersonRORC> ViewPeople { get; }

        public ReadOnlyReactiveCollection3ViewModel()
        {
            var roReactiveCollection = _model.People.ToReadOnlyReactiveCollection().AddTo(CompositeDisposable);

            DeleteItemCommand = new ReactiveCommand<PersonRORC>()
                .WithSubscribe(item => _model.DeleteItem(item), CompositeDisposable.Add);

            ViewPeople = roReactiveCollection.ToRoReactiveCollectionSelectedItemPair().AddTo(CompositeDisposable);
        }
    }

    class ReadOnlyReactiveCollection3Model : MyBindableBase
    {
        public ObservableCollection<PersonRORC> People { get; }

        public ReadOnlyReactiveCollection3Model()
        {
            var source = new PersonRORC[]
            {
                new("Jotaro"), new("Joseph"), new("Abdul"), new("Kakyoin"), new("Polnareff"), new("Iggy")
            };

            People = new(source);
        }

        public void DeleteItem(PersonRORC person)
        {
            _ = People.Remove(person);
        }
    }
}
