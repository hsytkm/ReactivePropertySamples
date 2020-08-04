using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
        public IReactiveProperty<string> InputText1 { get; }
        public IReadOnlyReactiveProperty<string> InputText1ErrorMessage { get; }

        [Required(ErrorMessage = "Please enter the characters!")]
        [StringLength(5, ErrorMessage = "Up to 5 characters")]
        public ReactiveProperty<string> InputText2 { get; }

        [Required(ErrorMessage = "Please enter the characters!")]
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

            InputText2 = new ReactiveProperty<string>()
                .SetValidateAttribute(() => InputText2)
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
                .WithSubscribe(() => Debug.WriteLine("Entry"), CompositeDisposable.Add);
        }
    }
}
