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
    public partial class ReactiveCollection2Page : Controls.MyPageControl
    {
        public ReactiveCollection2Page()
        {
            InitializeComponent();
            DataContext = new ReactiveCollection2ViewModel();
        }
    }

    class ReactiveCollection2ViewModel : MyDisposableBindableBase
    {
        private readonly Random _random = new Random();

        public ReactiveCollection<object> DataCollection { get; } = new ReactiveCollection<object>();

        #region Add
        public ICommand AddRandomIntCommand => _addRandomIntCommand ??=
            new MyCommand(() => DataCollection.AddOnScheduler(_random.Next(0, 101)));
        private ICommand _addRandomIntCommand;

        public ICommand AddRandomDoubleCommand => _addRandomDoubleCommand ??=
            new MyCommand(() => DataCollection.AddOnScheduler(_random.Next(0, 9999) / 10000.0));
        private ICommand _addRandomDoubleCommand;

        // 気まぐれで ReactiveCommand 実装
        public ReactiveCommand AddRandomTimeNowCommand { get; } = new ReactiveCommand();
        #endregion

        #region Insert
        public AsyncReactiveCommand InsertHeadRandomIntCommand { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand InsertHeadRandomDoubleCommand { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand InsertHeadRandomTimeNowCommand { get; } = new AsyncReactiveCommand();
        #endregion

        #region Remove/Clear
        public ReactiveCommand RemoveHeadItemCommand { get; }
        public AsyncReactiveCommand RemoveTailItemCommand { get; }
        public AsyncReactiveCommand ClearAllItemsCommand { get; }
        #endregion

        public ReactiveCollection2ViewModel()
        {
            #region Add
            AddRandomTimeNowCommand
                .Subscribe(() => DataCollection.AddOnScheduler(DateTime.Now.ToString()))
                .AddTo(CompositeDisposable);
            #endregion

            #region Insert
            InsertHeadRandomIntCommand
                .Subscribe(async _ =>
                {
                    // BackgroundスレッドからUIスレッド上で要素を追加する
                    await Task.Delay(500);
                    DataCollection.InsertOnScheduler(0, _random.Next(0, 101));
                })
                .AddTo(CompositeDisposable);

            InsertHeadRandomDoubleCommand
                .Subscribe(async _ =>
                {
                    await Task.Delay(500);
                    DataCollection.InsertOnScheduler(0, _random.Next(0, 9999) / 10000.0);
                })
                .AddTo(CompositeDisposable);

            InsertHeadRandomTimeNowCommand
                .Subscribe(async _ =>
                {
                    await Task.Delay(500);
                    DataCollection.InsertOnScheduler(0, DateTime.Now.ToString());
                })
                .AddTo(CompositeDisposable);
            #endregion

            #region Remove
            RemoveHeadItemCommand = DataCollection
                .CollectionChangedAsObservable()
                .Select(_ => DataCollection.Any())
                .ToReactiveCommand()
                .WithSubscribe(() => DataCollection.RemoveAtOnScheduler(0), CompositeDisposable.Add);

            RemoveTailItemCommand = DataCollection
                .CollectionChangedAsObservable()
                .Select(_ => DataCollection.Any())
                .ToAsyncReactiveCommand()
                .WithSubscribe(async () =>
                    {
                        await Task.Delay(500);
                        DataCollection.RemoveAtOnScheduler(DataCollection.Count - 1);
                    }, CompositeDisposable.Add);

            ClearAllItemsCommand = DataCollection
                .CollectionChangedAsObservable()
                .Select(_ => DataCollection.Any())
                .ToAsyncReactiveCommand()
                .WithSubscribe(async () =>
                    {
                        await Task.Delay(500);
                        DataCollection.ClearOnScheduler();
                    }, CompositeDisposable.Add);
            #endregion

            // Remove 達の CanExecute を false にするため、コレクションを操作する
            // ◆もっと良い実装ない？
            DataCollection.Clear();
        }
    }
}
