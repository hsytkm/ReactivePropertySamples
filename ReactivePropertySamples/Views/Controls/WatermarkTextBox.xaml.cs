#nullable disable
using System.Windows;
using System.Windows.Controls;

namespace ReactivePropertySamples.Views.Controls
{
    public partial class WatermarkTextBox : UserControl
    {
        // ウォーターマーク文字列
        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register(nameof(WatermarkText), typeof(string), typeof(WatermarkTextBox));
        public string WatermarkText
        {
            get => (string)GetValue(WatermarkTextProperty);
            set => SetValue(WatermarkTextProperty, value);
        }

        // 入力文字列
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(WatermarkTextBox));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public WatermarkTextBox()
        {
            InitializeComponent();
            this.GotFocus += (_, _) => textBox1.Focus();
        }
    }
}