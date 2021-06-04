using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ReactiveProperty3Page : Controls.MyPageControl
    {
        public ReactiveProperty3Page()
        {
            DataContext = new ReactiveProperty3ViewModel();
            InitializeComponent();
        }
    }

    /// <summary>
    /// 判定ルール
    ///  - ラストの \ は無視する
    ///  - IgnoreCase
    /// </summary>
    class FilePathEqualityComparer : EqualityComparer<string>
    {
        private static ReadOnlySpan<char> Revise(string source)
        {
            if (string.IsNullOrEmpty(source)) return source;
            var lower = source.ToLowerInvariant().AsSpan();
            return (lower[^1] == '\\') ? lower[0..^1] : lower;
        }

        public override bool Equals(string? x, string? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            if (x.Length != y.Length) return false;

            var span1 = Revise(x);
            var span2 = Revise(y);
            if (span1.Length != span2.Length) return false;

            for (int i = 0; i < span1.Length; i++)
            {
                if (span1[i] != span2[i]) return false;
            }
            return true;
        }

        public override int GetHashCode(string? obj)
            => (obj is null) ? 0 : Revise(obj).GetHashCode();
    }

    class ReactiveProperty3ViewModel : MyDisposableBindableBase
    {
        public IReactiveProperty<string> InputPath { get; } =
            new ReactiveProperty<string>(equalityComparer: new FilePathEqualityComparer());

        public ICommand SetStringCommand => _setStringCommand ??= new MyCommand<string>(x => InputPath.Value = x);
        private ICommand _setStringCommand = default!;

        public StringBuilder OutputTexts { get; } = new StringBuilder();

        public ReactiveProperty3ViewModel()
        {
            InputPath
                .Subscribe(x =>
                {
                    OutputTexts.Insert(0, x + Environment.NewLine);
                    NotifyPropertyChanged(nameof(OutputTexts));
                })
                .AddTo(CompositeDisposable);
        }
    }
}
