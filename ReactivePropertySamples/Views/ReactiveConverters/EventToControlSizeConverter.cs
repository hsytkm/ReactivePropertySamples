using System;
using System.Windows;
using Reactive.Bindings.Interactivity;

namespace ReactivePropertySamples.Views.ReactiveConverters
{
    class LoadedEventToControlSizeDelegateConverter : DelegateConverter<EventArgs, Size>
    {
        protected override Size OnConvert(EventArgs source)
        {
            if (AssociateObject is FrameworkElement fe)
                return new Size(fe.ActualWidth, fe.ActualHeight);

            throw new NotSupportedException();
        }
    }

    class SizeChangedEventToControlSizeDelegateConverter : DelegateConverter<SizeChangedEventArgs, Size>
    {
        protected override Size OnConvert(SizeChangedEventArgs source) => source.NewSize;
    }
}
