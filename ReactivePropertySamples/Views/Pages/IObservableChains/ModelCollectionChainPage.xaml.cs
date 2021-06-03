#nullable disable
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

        public IReactiveProperty<ModelCollectionFamily> SelectedFamily { get; } =
            new ReactiveProperty<ModelCollectionFamily>();

        public ReactiveCommand ClearSelectCommand { get; }
        public ReactiveCommand AddBabyCommand { get; }

        // 家族の子供たち。家族の選択状態に応じて、これを更新したい
        public ReadOnlyReactiveCollection<string> ChildrenSource
        {
            get => _childrenSource;
            private set
            {
                var oldItem = _childrenSource;
                if (SetProperty(ref _childrenSource, value))
                {
                    // 古いアイテムをDisposeする
                    if (oldItem is not null)
                    {
                        oldItem.Dispose();
                        if (CompositeDisposable.Contains(oldItem))
                            CompositeDisposable.Remove(oldItem);
                    }
                }
            }
        }
        private ReadOnlyReactiveCollection<string> _childrenSource;

        public ModelCollectionChainViewModel()
        {
            // 家族の選択を 未選択(null) に切り替えるコマンド
            ClearSelectCommand = SelectedFamily.Select(x => x is not null)
                .ToReactiveCommand()
                .WithSubscribe(() => SelectedFamily.Value = null, CompositeDisposable.Add);

            // 選択中の家族に子供を追加するコマンド
            AddBabyCommand = SelectedFamily.Select(x => x is not null)
                .ToReactiveCommand()
                .WithSubscribe(() => SelectedFamily.Value.AddChild(), CompositeDisposable.Add);

            // VM.Dispose() に備えて、CompositeDisposable に常に追加しておく
            // ◆これ以外の実装が思い浮かばない。もう少しシンプルにならんかなぁ。Disposeを考えたくない。
            SelectedFamily
                .Subscribe(x => ChildrenSource = x?.Children.ToReadOnlyReactiveCollection().AddTo(CompositeDisposable))
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
