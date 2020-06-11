using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class Validate1Page : Controls.MyPageControl
    {
        public Validate1Page()
        {
            InitializeComponent();
            DataContext = new Validate1ViewModel();
        }
    }

    class Validate1ViewModel : MyDisposableBindableBase
    {
        public ReactiveProperty<string> InputText1 { get; }
        public ReadOnlyReactiveProperty<string> InputText1ErrorMessage { get; }

        [Required(ErrorMessage = "Number is required")]
        public ReactiveProperty<string> Name { get; }

        [Required(ErrorMessage = "Number is required")]
        [RegularExpression("[0-9]+", ErrorMessage = "Please enter a valid number!")]
        [Range(0, 100)]
        public ReactiveProperty<string> Age { get; }

        public ReactiveCommand PersonEntryCommand { get; }

        public Validate1ViewModel()
        {
            InputText1 = new ReactiveProperty<string>()
                .SetValidateNotifyError(x => string.IsNullOrEmpty(x) ? "Please enter the characters!" : null)
                .AddTo(CompositeDisposable);

            InputText1ErrorMessage = InputText1
                .ObserveErrorChanged
                .Select(x => x?.Cast<string>()?.FirstOrDefault())   // 正常時は null が入る
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            Name = new ReactiveProperty<string>()
                .SetValidateAttribute(() => Name)
                .AddTo(CompositeDisposable);

            Age = new ReactiveProperty<string>()
                .SetValidateAttribute(() => Age)
                .AddTo(CompositeDisposable);

            PersonEntryCommand = new[]
                {
                    Name.ObserveHasErrors,
                    Age.ObserveHasErrors
                }
                .CombineLatestValuesAreAllFalse()
                .ToReactiveCommand()
                .AddTo(CompositeDisposable);
        }
    }
}
