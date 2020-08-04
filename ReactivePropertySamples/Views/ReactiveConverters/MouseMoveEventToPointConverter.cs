using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Reactive.Bindings.Interactivity;

namespace ReactivePropertySamples.Views.ReactiveConverters
{
    // ReactiveConverter は変換処理を Rx のメソッドチェーンで書ける
    class MouseMoveEventToPointConverter : ReactiveConverter<MouseEventArgs, Point>
    {
        protected override IObservable<Point> OnConvert(IObservable<MouseEventArgs> source)
        {
            return source
                .Select(x => x.GetPosition((IInputElement)this.AssociateObject));
        }
    }

    // DelegateConverter は変換処理を普通の C# のメソッドとして書ける
    class MouseMoveEventToPointDelegateConverter : DelegateConverter<MouseEventArgs, Point>
    {
        protected override Point OnConvert(MouseEventArgs source)
        {
            return source.GetPosition((IInputElement)this.AssociateObject);
        }
    }
}
