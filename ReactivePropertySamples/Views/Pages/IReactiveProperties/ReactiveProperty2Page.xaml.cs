#nullable enable
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ReactiveProperty2Page : Controls.MyPageControl
    {
        public ReactiveProperty2Page()
        {
            DataContext = new ReactiveProperty2ViewModel();
            InitializeComponent();
        }
    }

    class IgnoreCaseComparer : EqualityComparer<string>
    {
        public override bool Equals(string? x, string? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            return x.ToLowerInvariant() == y.ToLowerInvariant();
        }

        public override int GetHashCode(string? obj)
            => (obj is null) ? 0 : obj.ToLowerInvariant().GetHashCode();
    }

    class ReactiveProperty2ViewModel : MyDisposableBindableBase
    {
        // 大文字/小文字の差であれば、値を更新しない。変更通知が発行されない。
        // Can change comparer logic by the equalityComparer argument of constructor and factory methods.
        public IReactiveProperty<string> InputText { get; } =
            new ReactiveProperty<string>(equalityComparer: new IgnoreCaseComparer());

        public ICommand SetStringCommand => _setStringCommand ??= new MyCommand<string>(x => InputText.Value = x);
        private ICommand _setStringCommand = default!;

        public StringBuilder OutputTexts { get; } = new StringBuilder();

        public ReactiveProperty2ViewModel()
        {
            InputText
                .Subscribe(x =>
                {
                    OutputTexts.Insert(0, x + Environment.NewLine);
                    NotifyPropertyChanged(nameof(OutputTexts));
                })
                .AddTo(CompositeDisposable);
        }
    }
}
