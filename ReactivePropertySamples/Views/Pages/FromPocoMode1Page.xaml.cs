using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class FromPocoMode1Page : Controls.MyPageControl
    {
        public FromPocoMode1Page()
        {
            InitializeComponent();
            DataContext = new FromPocoMode1ViewModel();
        }
    }

    class FromPocoMode1ViewModel : MyDisposableBindableBase
    {
        private readonly static FromPocoMode1Model _model = new FromPocoMode1Model();

        public ReactiveProperty<string> PersonName { get; }

        public ICommand ChangeRpNameCommand => _changeRpNameCommand ??=
            new MyCommand<string>(x => PersonName.Value = x);
        private ICommand _changeRpNameCommand;

        public ICommand ChangeModelNameCommand => _changeModelNameCommand ??=
            new MyCommand<string>(x => _model.Name = x);
        private ICommand _changeModelNameCommand;

        public FromPocoMode1ViewModel()
        {
            PersonName = ReactiveProperty
                .FromObject(_model, x => x.Name)
                .AddTo(CompositeDisposable);

            PersonName
                .Subscribe(x => Debug.WriteLine($"ViewModel の ReactiveProperty.Value に \"{x}\" が設定されました。"))
                .AddTo(CompositeDisposable);
        }
    }

    class FromPocoMode1Model
    {
        public string Name
        {
            get => _name;
            set
            {
                if (!Equals(_name, value))
                {
                    _name = value;
                    Debug.WriteLine($"Model の Name プロパティに \"{value}\" が設定されました。");
                }
            }
        }
        private string _name = "No name";
    }
}
