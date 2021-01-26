using ReactivePropertySamples.Extensions;
using ReactivePropertySamples.Infrastructures;
using ReactivePropertySamples.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ReactivePropertySamples.Views
{
    public partial class PagesListBox : UserControl
    {
        public PagesListBox()
        {
            InitializeComponent();
            DataContext = new PagesListBoxViewModel();

            Loaded += (_, _) => filterTextBox.Focus();
        }
    }

    class PagesListBoxViewModel : MyBindableBase
    {
        public IReadOnlyCollection<PageSource> PagesSource => PageSourceStore.AllPageList;

        // リストの文字列による絞り込み
        public ICommand FilterCommand => _filterCommand ??= new MyCommand<string>(pattern =>
        {
            var collectionView = CollectionViewSource.GetDefaultView(PagesSource);
            collectionView.Filter = string.IsNullOrEmpty(pattern)
                ? default(Predicate<object>)                    // clear
                : x => (x as PageSource).IsContain(pattern);

            // フィルタリング後の先頭アイテムに移動させる
            if (collectionView.Cast<object>().FirstOrDefault() is PageSource page)
                SelectedPageSource = page;
        });
        private ICommand _filterCommand;

        // Viewで選択されたアイテム(こいつをContentに読み込む)
        public PageSource SelectedPageSource
        {
            get => _selectedPageSource;
            set
            {
                // 選択変更でViewコントロールを更新
                if (SetProperty(ref _selectedPageSource, value))
                {
                    if (value is not null)
                        TargetUserControl = (UserControl)Activator.CreateInstance(value.ControlType);
                }
            }
        }
        private PageSource _selectedPageSource;

        // Viewに表示するコントロール
        public UserControl TargetUserControl
        {
            get => _targetUserControl;
            private set => SetPropertyWithDispose(ref _targetUserControl, value);  // 古いInstanceをDisposeする版
        }
        private UserControl _targetUserControl;

    }

}
