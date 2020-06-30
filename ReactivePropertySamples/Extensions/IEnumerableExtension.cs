using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ReactivePropertySamples.Extensions
{
    static class IEnumerableExtension
    {
        /// <summary>
        /// 指定したコレクションからコピーされた要素を格納するObservableCollectionを生成
        /// </summary>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }

    }
}
