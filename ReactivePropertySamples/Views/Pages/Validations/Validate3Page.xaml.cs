using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class Validate3Page : Controls.MyPageControl
    {
        public Validate3Page()
        {
            InitializeComponent();
            DataContext = new Validate3ViewModel();
        }
    }

    public sealed class SumValuesAttribute : ValidationAttribute
    {
        private readonly int MaxValue;

        public SumValuesAttribute(int maxValue, string errorMessage)
            => (MaxValue, ErrorMessage) = (maxValue, errorMessage);

        public SumValuesAttribute(int maxValue) =>
            (MaxValue, ErrorMessage) = (maxValue, $"The total value should be {maxValue} or less.");

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // 想定外は成功扱い(他のルールに任せる)
            if (value is not IList<int> values) return ValidationResult.Success;

            return (values.Sum() <= MaxValue) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }

    class Validate3ViewModel : MyDisposableBindableBase
    {
        [Required(ErrorMessage = "Number is required")]
        [RegularExpression("[0-9]+", ErrorMessage = "Please enter a valid number!")]
        [Range(0, 100)]
        public ReactiveProperty<string> InputValue1 { get; }

        [Required(ErrorMessage = "Number is required")]
        [RegularExpression("[0-9]+", ErrorMessage = "Please enter a valid number!")]
        [Range(0, 100)]
        public ReactiveProperty<string> InputValue2 { get; }

        [SumValues(100)]
        public ReactiveProperty<IList<int>> CombineValidation { get; }
        public IReadOnlyReactiveProperty<string> CombineErrorMessage { get; }

        public ReactiveCommand ButtonCommand { get; }

        public Validate3ViewModel()
        {
            InputValue1 = new ReactiveProperty<string>(initialValue: "51")
                .SetValidateAttribute(() => InputValue1)
                .AddTo(CompositeDisposable);

            InputValue2 = new ReactiveProperty<string>(initialValue: "52")
                .SetValidateAttribute(() => InputValue2)
                .AddTo(CompositeDisposable);

            // 合計値のチェック
            CombineValidation =
                new[]
                {
                    InputValue1.ObserveHasErrors.Where(err => !err).Select(_ => int.Parse(InputValue1.Value)),
                    InputValue2.ObserveHasErrors.Where(err => !err).Select(_ => int.Parse(InputValue2.Value)),
                }
                .CombineLatest()
                .ToReactiveProperty()
                .SetValidateAttribute(() => CombineValidation)
                .AddTo(CompositeDisposable);

            // もう少しマシな Message の表示法があると思う
            CombineErrorMessage = CombineValidation.ObserveErrorChanged
                .Select(x => x?.Cast<string>()?.FirstOrDefault() ?? "")
                .ToReadOnlyReactivePropertySlim()
                .AddTo(CompositeDisposable);

            // バリデーションが通る(入力値が正常)な時のみコマンドを有効にする
            ButtonCommand = new[]
                {
                    InputValue1.ObserveHasErrors,
                    InputValue2.ObserveHasErrors,
                    CombineValidation.ObserveHasErrors,
                }
                .CombineLatestValuesAreAllFalse()
                .ToReactiveCommand()
                .AddTo(CompositeDisposable);
        }
    }
}
