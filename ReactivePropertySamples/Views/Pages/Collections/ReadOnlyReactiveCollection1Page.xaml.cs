using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        public ReadOnlyReactiveCollection<PersonRORC> People { get; }

        public ReactiveCommand<string> AddPersonNameToModelCommand { get; }
        public ReactiveCommand ClearModelPeopleCommand { get; }
        
        public ReadOnlyReactiveCollection1ViewModel()
        {
            var model = new ReadOnlyReactiveCollection1Model();

            // 元コレクション(Model)のデータ型を変換した新たなコレクションを作成
            People = model.People
                .ToReadOnlyReactiveCollection(x => new PersonRORC(x))
                .AddTo(CompositeDisposable);

            // Modelのコレクションにデータ追加
            AddPersonNameToModelCommand = new ReactiveCommand<string>()
                .WithSubscribe(x =>
                {
                    if (!string.IsNullOrEmpty(x)) model.AddPerson(x);
                },
                CompositeDisposable.Add);

            // Modelのコレクションのクリア
            ClearModelPeopleCommand = model.People.ObserveIsAny()
                .ToReactiveCommand(initialValue: false)
                .WithSubscribe(() => model.ClearPersons(), CompositeDisposable.Add);
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
