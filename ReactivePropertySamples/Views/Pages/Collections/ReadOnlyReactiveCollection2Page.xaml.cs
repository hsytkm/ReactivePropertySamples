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
    public partial class ReadOnlyReactiveCollection2Page : Controls.MyPageControl
    {
        public ReadOnlyReactiveCollection2Page()
        {
            InitializeComponent();
            DataContext = new ReadOnlyReactiveCollection2ViewModel();
        }
    }

    class ReadOnlyReactiveCollection2ViewModel : MyDisposableBindableBase
    {
        public ReadOnlyReactiveCollection<PersonRORC> People { get; }

        public ReactiveCommand<string> AddPersonNameToModelCommand { get; } = new ReactiveCommand<string>();
        public ReactiveCommand ClearModelPeopleCommand { get; } = new ReactiveCommand();
        
        public ReadOnlyReactiveCollection2ViewModel()
        {
            // ReactiveCommand<T>（IObservable<T>）から、ReadOnlyReactiveCollection を作成
            // onReset の指定により、Clear できるようにしている。
            People = AddPersonNameToModelCommand
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => new PersonRORC(x))
                .ToReadOnlyReactiveCollection(onReset: ClearModelPeopleCommand.ToUnit())
                .AddTo(CompositeDisposable);

            // 以下のように、コレクション要素がない場合に CanExecute=false にしたいが、
            // コレクションの作成に 本Command を指定する必要があり、玉子鶏理論により、やりたいことが書けない…
            // ◆何かよい実装ないかなぁ？ ReadOnlyReactiveCollection を辞めればスッと書けるけどサンプルにならないので。
            //ClearModelPeopleCommand = People.ObserveIsAny()
            //    .ToReactiveCommand()
            //    .AddTo(CompositeDisposable);
        }
    }

}
