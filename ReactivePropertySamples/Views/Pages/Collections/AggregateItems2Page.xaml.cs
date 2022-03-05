#nullable disable
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class AggregateItems2Page : Controls.MyPageControl
    {
        public AggregateItems2Page()
        {
            DataContext = new AggregateItems2ViewModel();
            InitializeComponent();
        }
    }

    class AggregateItems2ViewModel : MyDisposableBindableBase
    {
        private const int _layerCount = 3;
        public ReactiveCollection<Layer> Layers { get; }
        public ReadOnlyReactivePropertySlim<SelectableDesignerItemViewModelBase[]> AllItems { get; }
        public ReactiveCommand<int> AddLayterItemCommand { get; }
        public ReactiveCommand<int> RemoveLayterItemCommand { get; }

        public AggregateItems2ViewModel()
        {
            Layers = new ReactiveCollection<Layer>().AddTo(CompositeDisposable);

            // ここのコレクションのまとめ実装がヤバイ。2021.7時点の俺では思い浮かばへん…
            AllItems = Layers.CollectionChangedAsObservable()
                .Select(_ => Layers.Select(x => x.LayerItemsChangedAsObservable()).Merge())
                .Switch()
                .Select(_ => Layers.SelectMany(x => x.Items).Select(y => y.Item.Value).ToArray())
                .ToReadOnlyReactivePropertySlim(Array.Empty<SelectableDesignerItemViewModelBase>())
                .AddTo(CompositeDisposable);

            AddLayterItemCommand = new ReactiveCommand<int>()
                .WithSubscribe(x => Layers[x].Items.Add(new LayerItem()), CompositeDisposable.Add)
                .AddTo(CompositeDisposable);

            RemoveLayterItemCommand = new ReactiveCommand<int>()
                .WithSubscribe(x =>
                {
                    if (Layers[x].Items.Count > 0)
                        Layers[x].Items.RemoveAt(0);
                }, CompositeDisposable.Add)
                .AddTo(CompositeDisposable);

            InitLayers();
        }

        private void InitLayers()
        {
            for (var i = 0; i < _layerCount; i++)
            {
                var layer = new Layer();
                for (var j = 0; j < i; j++)
                {
                    layer.Items.Add(new LayerItem());
                }
                Layers.Add(layer);
            }
        }
    }

    class Layer : IDisposable
    {
        public ReactiveCollection<LayerItem> Items { get; } = new();

        public IObservable<Unit> LayerItemsChangedAsObservable() =>
            Items
                .ObserveElementObservableProperty(x => x.Item)
                .ToUnit()
                .Merge(Items
                    .CollectionChangedAsObservable()
                    .Where(x => x.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Reset)
                    .ToUnit());

        public void Dispose()
        {
            foreach (var item in Items) item.Dispose();
            Items.Dispose();
        }
    }

    class LayerItem : IDisposable
    {
        public ReactivePropertySlim<SelectableDesignerItemViewModelBase> Item { get; }

        public LayerItem()
        {
            var x = new SelectableDesignerItemViewModelBase();
            Item = new ReactivePropertySlim<SelectableDesignerItemViewModelBase>(x);
        }

        public void Dispose() => Item.Dispose();
    }

    class SelectableDesignerItemViewModelBase
    {
        private readonly DateTime _time = DateTime.Now;
        public override string ToString() => _time.ToString("HH:mm:ss:ffff");
    }
}
