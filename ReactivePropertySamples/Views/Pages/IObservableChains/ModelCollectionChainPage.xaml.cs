using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.ObjectExtensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ModelCollectionChainPage : Controls.MyPageControl
    {
        public ModelCollectionChainPage()
        {
            InitializeComponent();
            DataContext = new ModelCollectionChainViewModel();
        }
    }

    class ModelCollectionChainViewModel : MyDisposableBindableBase
    {
        // 選択候補の家族ら  ※家族は増えない仕様なので Array にとしく
        public ModelCollectionFamily[] FamiliesSource { get; } = new[]
        {
            new ModelCollectionFamily("Tanaka", "tarou", "jirou"),
            new ModelCollectionFamily("Suzuki", "hanako"),
            new ModelCollectionFamily("Fujita"),
        };

        public ReactiveProperty<ModelCollectionFamily> SelectedFamily { get; } =
            new ReactiveProperty<ModelCollectionFamily>();

        public ReactiveCommand ClearSelectCommand { get; }
        public ReactiveCommand AddBabyCommand { get; }

        // 家族の子供たち。家族の選択状態に応じて、これを更新したい
        public ReactiveCollection<string> ChildrenSource
        {
            get => _childrenSource;
            private set => SetProperty(ref _childrenSource, value);
        }
        private ReactiveCollection<string> _childrenSource;

        public ModelCollectionChainViewModel()
        {
            // 家族の選択を 未選択(null) に切り替えるコマンド
            ClearSelectCommand = SelectedFamily.Select(x => x != null)
                .ToReactiveCommand()
                .WithSubscribe(() => SelectedFamily.Value = null, CompositeDisposable.Add);

            // 選択中の家族に子供を追加するコマンド
            AddBabyCommand = SelectedFamily.Select(x => x != null)
                .ToReactiveCommand()
                .WithSubscribe(() => SelectedFamily.Value.AddChild(), CompositeDisposable.Add);

            // このノリで実装したいけど、入れ子になるのでビルド通らない
            //ChildrenSource = SelectedFamily
            //    .Select(x => x.Children)
            //    .ToReadOnlyReactiveCollection()
            //    .AddTo(CompositeDisposable);

            // これ以外の実装が思い浮かばない。Dipsoseとか無理やりで納得いっていない…
            //SelectedFamily
            //    .Do(_ => ChildrenSource?.Dispose())
            //    .Do(_ => ChildrenSource = null)
            //    .Where(x => x != null)
            //    .Select(x => ChildrenSource = x.Children.ToReadOnlyReactiveCollection().AddTo(CompositeDisposable))
            //    .Subscribe()
            //    .AddTo(CompositeDisposable);

            // これやと選択変化時は更新されるけど、変更通知が来ないしなぁ…
            ChildrenSource = SelectedFamily
                .Select(x => x?.Children)
                .Do(x => ChildrenSource?.Clear())
                .Where(x => x != null)
                .SelectMany(x => x)
                .ToReactiveCollection()
                .AddTo(CompositeDisposable);

        }
    }

    class ModelCollectionFamily
    {
        public string Name { get; }
        public ObservableCollection<string> Children { get; }
        public ModelCollectionFamily(string name, params string[] children)
        {
            Name = name;
            Children = new ObservableCollection<string>(children);
        }

        public void AddChild() => Children.Add("baby");
    }
}
