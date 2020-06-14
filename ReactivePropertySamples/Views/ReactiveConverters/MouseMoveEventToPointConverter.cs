using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Reactive.Bindings.Interactivity;

namespace ReactivePropertySamples.Views.ReactiveConverters
{
    class MouseMoveEventToPointConverter : ReactiveConverter<MouseEventArgs, Point>
    {
        protected override IObservable<Point> OnConvert(IObservable<MouseEventArgs> source)
        {
            return source
                .Select(x => x.GetPosition((IInputElement)this.AssociateObject));
        }
    }
}
