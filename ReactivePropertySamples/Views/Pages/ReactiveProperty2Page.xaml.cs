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
            InitializeComponent();
            DataContext = new ReactiveProperty2ViewModel();
        }
    }

    class IgnoreCaseComparer : EqualityComparer<string>
    {
        public override bool Equals(string x, string y)
            => x?.ToLower() == y?.ToLower();

        public override int GetHashCode(string obj)
            => (obj?.ToLower()).GetHashCode();
    }

    class ReactiveProperty2ViewModel : MyDisposableBindableBase
    {
        // Can change comparer logic by the equalityComparer argument of constructor and factory methods.
        public ReactiveProperty<string> InputText { get; } =
            new ReactiveProperty<string>(equalityComparer: new IgnoreCaseComparer());

        public ICommand SetStringCommand => _setStringCommand ??=
            new MyCommand<string>(x => InputText.Value = x);
        private ICommand _setStringCommand;

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
