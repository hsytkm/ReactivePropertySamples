using ReactivePropertySamples.Views.Controls;
using ReactivePropertySamples.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePropertySamples.Views
{
    static class PageSourceStore
    {
        // Viewに表示されるコントロールのリスト
        public static IReadOnlyCollection<PageSource> AllPageList { get; } = new PageSource[]
        {
            /* 未対応
             *  CatchIgnore ErrorChangedAsObservable 
             *  ObserveErrorInfo OnErrorRetry
             */
            //new(typeof(Page)),

            // Collections
            new(typeof(ReactiveCollection1Page)),
            new(typeof(ReactiveCollection2Page)),
            new(typeof(ReadOnlyReactiveCollection1Page)),
            new(typeof(ReadOnlyReactiveCollection2Page)),
            new(typeof(ReadOnlyReactiveCollection3Page)),
            new(typeof(IFilteredReadOnlyObservableCollection1Page)),
            new(typeof(ObserveElementProperty1Page)),
            new(typeof(ObserveElementPropertyChanged1Page)),
            new(typeof(AggregateItems1Page)),
            new(typeof(AggregateItems2Page)),

            // Commands
            new(typeof(ReactiveCommand1Page)),
            new(typeof(AsyncReactiveCommand1Page)),
            new(typeof(CanExecuteChangedAsObservablePage)),
            new(typeof(TimerStartCommandPage)),

            // IObservableChains
            new(typeof(CombineAllValuePage)),
            new(typeof(AsyncTaskChainPage)),
            new(typeof(ModelCollectionChainPage)),
            new(typeof(ObservableCollectionScanPage)),

            // IReactiveProperties
            new(typeof(ReactiveProperty1Page)),
            new(typeof(ReactiveProperty2Page)),
            new(typeof(ReactiveProperty3Page)),
            new(typeof(ObserveProperty1Page)),
            new(typeof(FromPocoMode1Page)),
            new(typeof(ReactivePropertySlim1Page)),
            new(typeof(DisposePreviousValuePage)),

            // Notifiers
            new(typeof(BooleanNotifier1Page)),
            new(typeof(BooleanNotifier2Page)),
            new(typeof(BusyNotifierPage)),
            new(typeof(CountNotifierPage)),
            new(typeof(MessageBroker1Page)),
            new(typeof(AsyncMessageBroker1Page)),
            new(typeof(ScheduledNotifierPage)),
            new(typeof(ScheduledNotifierLoggerPage)),

            // EventToReactive
            new(typeof(EventToReactiveProperty1Page)),
            new(typeof(EventToReactiveProperty2Page)),
            new(typeof(EventToReactiveProperty3Page)),
            new(typeof(EventToReactiveProperty4Page)),
            new(typeof(EventToReactiveProperty5Page)),
            new(typeof(EventToReactiveCommand1Page)),

            // Synchronizations
            new(typeof(TwoWay1Page)),
            new(typeof(TwoWay2Page)),

            // Validations
            new(typeof(Validate1Page)),
            new(typeof(Validate2Page)),
            new(typeof(Validate3Page)),

            // Samples
            new(typeof(SelectRectanglePage)),
            new(typeof(PeriodicInvokePage)),
            new(typeof(FpsCounterPage)),
            new(typeof(ThrottleFirstPage)),
            new(typeof(DragThumbPage)),
            
            //new(typeof(BlankPage), "Title", "Subtitle", "Description"),
        };
    }

    class PageSource
    {
        public string Title { get; } = "";
        public string Subtitle { get; } = "";
        public string Description { get; } = "";
        public IReadOnlyCollection<string>? Keywords { get; }
        public Type ControlType { get; }
        public string TypeName { get; }

        public PageSource(Type controlType)
        {
            ControlType = controlType;
            TypeName = controlType.ToString().Split('.')[^1];        // Typeから名前空間を除去する

            // 一度インスタンス化して、内部情報を吸い上げる
            if (Activator.CreateInstance(controlType) is MyPageControl control)
            {
                Title = control.Title;
                Subtitle = control.Subtitle;
                Description = control.Description;
                Keywords = control.Keywords;

                control.Dispose();  // これ大事！
            }
        }

        public PageSource(Type controlType, string title, string subTitle, string description)
            : this(controlType)
        {
            Title = title;
            Subtitle = subTitle;
            Description = description;
        }

        // 指定文字列がfieldに含まれるか判定する
        public bool IsContain(string pat)
        {
            pat = pat.ToLowerInvariant();

            var ss = new[] { Title, Subtitle, Description, TypeName };
            if (ss.Any(x => x.ToLowerInvariant().Contains(pat))) return true;

            return Keywords is null ? false : Keywords.Any(x => x.ToLowerInvariant().Contains(pat));
        }
    }
}
