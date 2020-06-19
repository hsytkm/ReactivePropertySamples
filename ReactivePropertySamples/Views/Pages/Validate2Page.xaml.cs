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
    public partial class Validate2Page : Controls.MyPageControl
    {
        public Validate2Page()
        {
            InitializeComponent();
            DataContext = new Validate2ViewModel();
        }
    }

    // stringの偶数チェック
    sealed class MyIntEvenValidationAttribute : ValidationAttribute
    {
        public MyIntEvenValidationAttribute(string errorMessage)
            : base(errorMessage) { }

        public override bool IsValid(object value)
            => int.TryParse(value.ToString(), out var x) && ((x & 1) == 0);
    }

    class Validate2ViewModel : MyDisposableBindableBase
    {
        [MyIntEvenValidation("偶数を入力してね")]
        public ReactiveProperty<string> IntValue { get; } = new ReactiveProperty<string>();

        public ReactiveCommand ButtonCommand { get; }

        public Validate2ViewModel()
        {
            IntValue = new ReactiveProperty<string>(initialValue: "1")
                .SetValidateAttribute(() => IntValue)
                .AddTo(CompositeDisposable);

            // バリデーションが通る(入力値が正常)な時のみコマンドを有効にする
            ButtonCommand = IntValue.ObserveHasErrors
                .Inverse()
                .ToReactiveCommand()
                .AddTo(CompositeDisposable);

        }
    }
}
