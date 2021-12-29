using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace ReactivePropertySamples.ViewModels
{
    /// <summary>
    /// ReadOnlyReactiveCollection から SelectedItem を管理します。
    /// 元コレクションの要素が存在しなければ、SelectedItem は default(null) になります。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class RoReactiveCollectionSelectedItemPair<T> : IDisposable
    {
        private bool _disposed;
        private readonly CompositeDisposable _disposables = new();

        public ReadOnlyReactiveCollection<T> Collection { get; }
        public IReactiveProperty<T?> SelectedItem { get; }

        public RoReactiveCollectionSelectedItemPair(ReadOnlyReactiveCollection<T> roReactiveCollection)
        {
            Collection = roReactiveCollection;

            var initialValue = Collection.Count > 0 ? Collection[0] : default;
            SelectedItem = new ReactivePropertySlim<T?>(initialValue).AddTo(_disposables);

            var selectedItemIndex = SelectedItem
                .Select(item => item is null ? -1 : Collection.IndexOf(item))
                .ToReadOnlyReactivePropertySlim()
                .AddTo(_disposables);

            // 未選択なら要素追加で初期要素を選択します
            Collection.ObserveAddChanged()
                .Subscribe(addItem =>
                {
                    if (SelectedItem.Value is null)
                        SelectedItem.Value = addItem;
                })
                .AddTo(_disposables);

            // 選択中なら要素削除で要素を変更します(選択の1つ前の要素)
            Collection.ObserveRemoveChanged()
                .Subscribe(removeItem =>
                {
                    if (Collection.Count == 0)
                    {
                        SelectedItem.Value = default;
                    }
                    else if (removeItem is not null && removeItem.Equals(SelectedItem.Value))
                    {
                        var newIndex = Math.Clamp(selectedItemIndex.Value - 1, 0, Collection.Count);
                        SelectedItem.Value = Collection[newIndex];
                    }
                })
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposables.Dispose();
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }
    }

    public static class RoReactiveCollectionSelectedItemPairExtensions
    {
        public static RoReactiveCollectionSelectedItemPair<T> ToRoReactiveCollectionSelectedItemPair<T>(this ReadOnlyReactiveCollection<T> collection)
            => new RoReactiveCollectionSelectedItemPair<T>(collection);
    }
}
