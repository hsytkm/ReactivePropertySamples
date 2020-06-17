using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ReadOnlyReactiveCollection1Page : Controls.MyPageControl
    {
        public ReadOnlyReactiveCollection1Page()
        {
            InitializeComponent();
            DataContext = new ReadOnlyReactiveCollection1ViewModel();
        }
    }

    class ReadOnlyReactiveCollection1ViewModel : MyDisposableBindableBase
    {
        private readonly ReadOnlyReactiveCollection1Model _model = new ReadOnlyReactiveCollection1Model();
        public ReadOnlyReactiveCollection<PersonRORC> People { get; }

        public ReactiveCommand<string> AddPersonNameToModelCommand { get; } = new ReactiveCommand<string>();
        public ReactiveCommand ClearModelPeopleCommand { get; } = new ReactiveCommand();
        
        public ReadOnlyReactiveCollection1ViewModel()
        {
            // 元コレクションのデータ型を変換した新たなコレクションを作成
            People = _model.People
                .ToReadOnlyReactiveCollection(x => new PersonRORC(x))
                .AddTo(CompositeDisposable);

            AddPersonNameToModelCommand
                .Where(x => !string.IsNullOrEmpty(x))
                .Subscribe(x => _model.AddPerson(x))
                .AddTo(CompositeDisposable);

            // Collection.Any() による CanExecute 切り替えは未実装
            ClearModelPeopleCommand
                .Subscribe(() => _model.ClearPersons())
                .AddTo(CompositeDisposable);
        }
    }

    class PersonRORC : MyBindableBase
    {
        public string Name { get; }
        public PersonRORC(string name) => Name = name;
    }

    class ReadOnlyReactiveCollection1Model : MyBindableBase
    {
        public ObservableCollection<string> People { get; } = new ObservableCollection<string>();

        public void AddPerson(string name) => People.Add(name);
        public void ClearPersons() => People.Clear();
    }
}
