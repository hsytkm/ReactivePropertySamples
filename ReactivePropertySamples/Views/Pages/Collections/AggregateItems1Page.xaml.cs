#nullable disable
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class AggregateItems1Page : Controls.MyPageControl
    {
        public AggregateItems1Page()
        {
            DataContext = new AggregateItems1ViewModel();
            InitializeComponent();
        }
    }

    class ItemAg : MyBindableBase
    {
        public string GroupName { get; }
        public Color Color
        {
            get => _color;
            private set => SetProperty(ref _color, value);
        }
        private Color _color;
        public ItemAg(Color c, string n) => (Color, GroupName) = (c, n);
        public ICommand ReverseCommand => _reverseCommand ??=
            new MyCommand(() => Color = Color.FromRgb((byte)(Color.R ^ 0xFF), (byte)(Color.G ^ 0xFF), (byte)(Color.B ^ 0xFF)));
        private ICommand _reverseCommand;
        public override string ToString() => Color.ToString();
    }

    class ParentAg : MyBindableBase
    {
        public string Name { get; }
        public ObservableCollection<ItemAg> Items { get; } = new ObservableCollection<ItemAg>();
        public ParentAg(string name) => Name = name;
        public void AddColor(Color color) => Items.Add(new ItemAg(color, Name));
        private static readonly Random _random = new Random();
        private static Color GetRandomColor()
        {
            static byte GetRandomByte() => (byte)_random.Next(256);
            return Color.FromRgb(GetRandomByte(), GetRandomByte(), GetRandomByte());
        }
        public ICommand AddItemCommand => _addItemCommand ??= new MyCommand(() => AddColor(GetRandomColor()));
        private ICommand _addItemCommand;
        public ICommand ClearItemsCommand => _clearItemsCommand ??= new MyCommand(() =>
            {
                // Clear() だと何が消えたか把握できないので1つずつ消す
                for (int i = Items.Count - 1; i >= 0; --i) Items.RemoveAt(i);
            });
        private ICommand _clearItemsCommand;
        public override string ToString() => Name;
    }

    class AggregateItems1ViewModel : MyDisposableBindableBase
    {
        // 各Parent の Items(Color) を Aggregate して、1つの Items にする
        public ObservableCollection<ItemAg> AggregateItems { get; } = new ObservableCollection<ItemAg>();

        public ObservableCollection<ParentAg> Parents { get; }

        public AggregateItems1ViewModel()
        {
            var parentRed = CreateParentInstance("Group 1");
            var parentGreen = CreateParentInstance("Group 2");
            var parentBlue = CreateParentInstance("Group 3");

            var parents = new[] { parentRed, parentGreen, parentBlue };

            parentRed.AddColor(Colors.Red);
            parentRed.AddColor(Colors.Coral);
            parentRed.AddColor(Colors.Salmon);

            parentGreen.AddColor(Colors.Green);
            parentGreen.AddColor(Colors.DarkGreen);

            parentBlue.AddColor(Colors.Blue);
            parentBlue.AddColor(Colors.SkyBlue);
            parentBlue.AddColor(Colors.SlateBlue);

            Parents = new ObservableCollection<ParentAg>(parents);
        }

        // インスタンスを作成して、コレクション変化イベントをアタッチする
        private ParentAg CreateParentInstance(string name)
        {
            var parent = new ParentAg(name);

            parent.Items
                .ObserveAddChangedItems()
                .Subscribe(addItems =>
                {
                    foreach (var item in addItems)
                    {
                        AggregateItems.Add(item);
                    }
                })
                .AddTo(CompositeDisposable);

            parent.Items
                .ObserveRemoveChangedItems()
                .Subscribe(removeItems =>
                {
                    foreach (var item in removeItems)
                    {
                        AggregateItems.Remove(item);
                    }
                })
                .AddTo(CompositeDisposable);

            return parent;
        }
    }
}
