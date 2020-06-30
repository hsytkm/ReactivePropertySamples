using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ReactiveCollection1Page : Controls.MyPageControl
    {
        public ReactiveCollection1Page()
        {
            InitializeComponent();
            DataContext = new ReactiveCollection1ViewModel();
        }
    }

    class ReactiveCollection1ViewModel : MyDisposableBindableBase
    {
        public ReactiveCollection<string> SourceValues { get; }
        public ReactiveCommand AddCommand { get; }
        public ReactiveCommand RemoveCommand { get; }
        public ReactiveCommand ClearCommand { get; }

        public ReactiveCollection1ViewModel()
        {
            AddCommand = new ReactiveCommand().AddTo(CompositeDisposable);

            // ReactiveCommand（正確には IObservable<T>） から ReactiveCollection<T> を作成
            // この場合やったら、ReadOnlyReactiveCollection<T> の方がスッキリする。我ながら例が悪い。
            SourceValues = AddCommand
                .Select(_ => DateTime.Now.ToString("hh:mm:ss.ff"))
                .ToReactiveCollection()
                .AddTo(CompositeDisposable);

            // ReactiveCollection<T> から ReactiveCommand を作成
            RemoveCommand = SourceValues
                .CollectionChangedAsObservable()
                .Select(_ => SourceValues.Any())
                .ToReactiveCommand(initialValue: false)
                .WithSubscribe(() => SourceValues.RemoveAt(0), CompositeDisposable.Add);

            ClearCommand = SourceValues
                .CollectionChangedAsObservable()
                .Select(_ => SourceValues.Any())
                .ToReactiveCommand(initialValue: false)
                .WithSubscribe(() => SourceValues.Clear(), CompositeDisposable.Add);

            // Remove 達の CanExecute を false にするため、コレクションを操作する
            // ◆もっと良い実装ない？  ⇒  initialValue: false の実装に変えた。
            //SourceValues.Clear();    
        }
    }
}
