using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace ReactivePropertySamples.Extensions
{
    static class ObservableCollectionExtension
    {
        /// <summary>
        /// CountプロパティをIObservableとして購読する
        /// </summary>
        public static IObservable<int> ObserveCount<T>(this ObservableCollection<T> source) =>
            source.ObserveProperty(x => x.Count);

        /// <summary>
        /// シークエンスに要素が含まれているかをIObservableとして購読する
        /// </summary>
        public static IObservable<bool> ObserveIsAny<T>(this ObservableCollection<T> source) =>
            source.ObserveCount().Select(x => x >= 1);

        /// <summary>
        /// シークエンスが空かをIObservableとして購読する
        /// </summary>
        public static IObservable<bool> ObserveIsEmpty<T>(this ObservableCollection<T> source) =>
            source.ObserveCount().Select(x => x <= 0);
    }
}
