using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class TwoWay2Page : Controls.MyPageControl
    {
        public TwoWay2Page()
        {
            InitializeComponent();
            DataContext = new TwoWay2ViewModel();
        }
    }

    class TwoWay2ViewModel : MyDisposableBindableBase
    {
        private readonly TwoWay2Model _model = new TwoWay2Model();

        [Required(ErrorMessage ="Number is required")]
        [RegularExpression("[0-9]+$", ErrorMessage = "Please enter a valid number!")]
        [Range(0, 100)]
        public ReactiveProperty<string> InputInt2 { get; }

        [Required(ErrorMessage = "Number is required")]
        [RegularExpression("^[0-9]+(\\.[0-9]{1,2})?$", ErrorMessage = "Please enter a valid number!")]
        [Range(1.0, 99.99)]
        public ReactiveProperty<string> InputDouble2 { get; }

        public IReadOnlyReactiveProperty<int> CheckModelValueInt2 { get; }
        public IReadOnlyReactiveProperty<double> CheckModelValueDouble2 { get; }

        public TwoWay2ViewModel()
        {
            // ignoreValidationErrorValue=true なら検証エラー時、Model に値を書き込まない
            InputInt2 = _model.ToReactivePropertyAsSynchronized(
                    x => x.UserInt2,
                    convert: m => m.ToString(),
                    convertBack: vm => int.Parse(vm),
                    ignoreValidationErrorValue: true)
                .SetValidateAttribute(() => InputInt2)
                .AddTo(CompositeDisposable);

            InputDouble2 = _model.ToReactivePropertyAsSynchronized(
                    x => x.UserDouble2,
                    convert: m => m.ToString(),
                    convertBack: vm => double.Parse(vm),
                    ignoreValidationErrorValue: true)
                .SetValidateAttribute(() => InputDouble2)
                .AddTo(CompositeDisposable);


            // 以下、確認用にModelの値を別口で参照
            CheckModelValueInt2 = _model.ObserveProperty(x => x.UserInt2)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);

            CheckModelValueDouble2 = _model.ObserveProperty(x => x.UserDouble2)
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
        }
    }

    class TwoWay2Model : MyBindableBase
    {
        public int UserInt2
        {
            get => _userInt2;
            set => SetProperty(ref _userInt2, value);
        }
        private int _userInt2;

        public double UserDouble2
        {
            get => _userDouble2;
            set => SetProperty(ref _userDouble2, value);
        }
        private double _userDouble2;
    }
}
