using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class DisposePreviousValuePage : Controls.MyPageControl
    {
        public DisposePreviousValuePage()
        {
            InitializeComponent();
            DataContext = new DisposePreviousValueViewModel();
        }
    }

    class DisposePreviousValueViewModel : MyDisposableBindableBase
    {
        public IReadOnlyCollection<string> Items { get; } = Enumerable.Range(0, 5).Select(x => $"Data{x}").ToArray();
        public IReactiveProperty<string> SelectedItem { get; }

        public StringBuilder LogBuilder { get; } = new StringBuilder();

        public ICommand AddLogCommand => _addLogCommand ??=
            new MyCommand<string>(message =>
            {
                LogBuilder.AppendLine(message);
                NotifyPropertyChanged(nameof(LogBuilder));
            });
        private ICommand _addLogCommand = default!;

        public DisposePreviousValueViewModel()
        {
            SelectedItem = new ReactiveProperty<string>(mode: ReactivePropertyMode.DistinctUntilChanged)
                .AddTo(CompositeDisposable);

            var hituyounai_kedo_teigi = SelectedItem
                .Select(x => new DisposePreviousValueData(AddLogCommand, x))
                .DisposePreviousValue()         // 元の値を Dispose()
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
        }
    }

    class DisposePreviousValueData : IDisposable
    {
        private readonly ICommand _command;
        private readonly string _name;
        public DisposePreviousValueData(ICommand command, string name)
        {
            _command = command;
            _name = name;
            _command.Execute($"Add {_name}");
        }

        public void Dispose()
        {
            _command.Execute($"Dispose {_name}");
        }
    }
}
