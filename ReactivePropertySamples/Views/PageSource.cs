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
        public static IList<PageSource> AllPageList { get; } = new[]
        {
            // 未対応 CatchIgnore CanExecuteChangedAsObservable ErrorChangedAsObservable
            // 未対応 ObserveErrorInfo Inverse Pairwise OnErrorRetry ToUnit ScheduledNotifier
            // 未対応 IFilteredReadOnlyObservableCollection ReactivePropertySlim
            // 未対応 ReactiveConverter

            //new PageSource(typeof(ObserveElementProperty1Page)),

            // 未対応  
            new PageSource(typeof(ObserveElementPropertyChanged1Page)),
            new PageSource(typeof(ObserveElementProperty1Page)),
            new PageSource(typeof(ReadOnlyReactiveCollection1Page)),
            // 未対応

            new PageSource(typeof(ReactiveCollection1Page)),
            new PageSource(typeof(TwoWay2Page)),
            new PageSource(typeof(Validate1Page)),
            new PageSource(typeof(CountNotifierPage)),
            new PageSource(typeof(ReactiveCommand1Page)),
            new PageSource(typeof(BusyNotifierPage)),
            new PageSource(typeof(FromPocoMode1Page)),
            new PageSource(typeof(CombineAllValuePage)),
            new PageSource(typeof(BooleanNotifierPage)),
            new PageSource(typeof(TwoWay1Page)),
            new PageSource(typeof(AsyncReactiveCommand1Page)),
            new PageSource(typeof(ObserveProperty1Page)),
            new PageSource(typeof(ReactiveProperty1Page)),

            new PageSource(typeof(BlankPage), "Title", "Subtitle", "Description"),
        }
        .ToList();
    }

    class PageSource
    {
        public string Title { get; }
        public string Subtitle { get; }
        public string Description { get; }
        public IList<string> Keywords { get; }
        public Type ControlType { get; }
        public string TypeName { get; }

        public PageSource(Type controlType)
        {
            ControlType = controlType;
            TypeName = controlType.ToString().Split('.').Last();        // Typeから名前空間を除去する

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
            pat = pat.ToLower();

            var ss = new[] { Title, Subtitle, Description, TypeName };
            if (ss.Any(x => x.ToLower().Contains(pat))) return true;

            if (Keywords != null) return Keywords.Any(x => x.ToLower().Contains(pat));
            return false;
        }
    }
}
