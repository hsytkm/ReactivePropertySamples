using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ReactivePropertySamples.Views.Controls
{
    public class MyPageControl : DisposableMyControl
    {
        // タイトル
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(MyPageControl));
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // サブタイトル
        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(MyPageControl));
        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        // 説明文
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(MyPageControl));
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        // キーワードリスト
        public static readonly DependencyProperty KeywordsProperty =
            DependencyProperty.Register(nameof(Keywords), typeof(IReadOnlyCollection<string>), typeof(MyPageControl));
        public IReadOnlyCollection<string> Keywords
        {
            get => (IReadOnlyCollection<string>)GetValue(KeywordsProperty);
            set => SetValue(KeywordsProperty, value);
        }

        public MyPageControl()
        {
            // 型名から名前空間を削除して、クラス名のPageを削除
            Title = this.GetType().ToString().Split('.')[^1].Replace("Page", "");
        }
    }
}
