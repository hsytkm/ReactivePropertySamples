using ReactivePropertySamples.Views.Controls;
using ReactivePropertySamples.Views.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReactivePropertySamples.Views
{
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


        // Viewに表示されるコントロールのリスト
        public static IList<PageSource> PageList { get; } = new[]
        {
            new PageSource(typeof(ObserveProperty1Page)),
            new PageSource(typeof(ReactiveProperty1Page)),
            new PageSource(typeof(BlankPage), "Title", "Subtitle", "Description"),
        }
        .ToList();
    }
}
