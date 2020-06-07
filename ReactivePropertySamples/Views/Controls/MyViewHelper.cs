using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ReactivePropertySamples.Views.Controls
{
    static class MyViewHelper
    {
        /// <summary>
        /// 指定コントロールの子孫要素を全てDispose取得
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static void DisposeDescendants(this DependencyObject d)
        {
            foreach (var child in d.GetDescendants())
            {
                if (child is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        /// <summary>
        /// 指定コントロールの子孫要素を取得  https://blog.xin9le.net/entry/2013/10/29/222336
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject d)
        {
            if (d is null) throw new ArgumentNullException(nameof(d));

            foreach (var child in Children(d))
            {
                yield return child;
                foreach (var grandChild in child.GetDescendants())
                    yield return grandChild;
            }

            static IEnumerable<DependencyObject> Children(DependencyObject dependencyObject)
            {
                var count = VisualTreeHelper.GetChildrenCount(dependencyObject);
                if (count == 0)
                    yield break;

                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(dependencyObject, i);
                    if (child != null)
                        yield return child;
                }
            }
        }

    }
}
