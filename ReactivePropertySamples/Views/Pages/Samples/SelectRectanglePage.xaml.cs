using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactivePropertySamples.Infrastructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class SelectRectanglePage : Controls.MyPageControl
    {
        public SelectRectanglePage()
        {
            InitializeComponent();
            DataContext = new SelectRectangleViewModel();
        }
    }

    class SelectRectangleViewModel : MyDisposableBindableBase
    {
        public BitmapSource MyImage { get; } = BitmapFrame.Create(new Uri("pack://application:,,,/ReactivePropertySamples;component/Assets/Image1.jpg"));
        public IReactiveProperty<Point> MouseDownPoint { get; }
        public IReactiveProperty<Unit> MouseUpUnit { get; }
        public IReactiveProperty<Point> MouseMovePoint { get; }
        public IReactiveProperty<Size> ViewImageSize { get; }
        public IReactiveProperty<bool> IsFinishedSelectingRectangle { get; }

        /// <summary>Viewの座標系</summary>
        public IReactiveProperty<Rect> SelectedRectangle { get; }

        /// <summary>原画像の座標系</summary>
        public IReactiveProperty<Rect> FixedRectangle { get; }

        public SelectRectangleViewModel()
        {
            MouseDownPoint = new ReactivePropertySlim<Point>(mode: ReactivePropertyMode.None).AddTo(CompositeDisposable);
            MouseUpUnit = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None).AddTo(CompositeDisposable);
            MouseMovePoint = new ReactivePropertySlim<Point>().AddTo(CompositeDisposable);
            ViewImageSize = new ReactivePropertySlim<Size>().AddTo(CompositeDisposable);
            IsFinishedSelectingRectangle = new ReactivePropertySlim<bool>().AddTo(CompositeDisposable);
            SelectedRectangle = new ReactivePropertySlim<Rect>().AddTo(CompositeDisposable);
            FixedRectangle = new ReactivePropertySlim<Rect>().AddTo(CompositeDisposable);
            var draggingVector = new ReactivePropertySlim<Vector>().AddTo(CompositeDisposable);

            // マウス操作開始時の初期化
            MouseDownPoint
                .Subscribe(_ =>
                {
                    IsFinishedSelectingRectangle.Value = false;
                    draggingVector.Value = default;
                })
                .AddTo(CompositeDisposable);

            // マウス操作中に移動量を流す + 操作完了時に枠位置を通知する
            MouseMovePoint
                .Pairwise()
                .Select(x => x.NewItem - x.OldItem)
                .SkipUntil(MouseDownPoint.ToUnit())
                .TakeUntil(MouseUpUnit)
                .Finally(() =>
                {
                    //Debug.WriteLine($"LeftTop: {MouseDownPoint.Value:f2}, Vector: {draggingVector:f2}");
                    if (draggingVector.Value == default) return;

                    // 枠の確定
                    IsFinishedSelectingRectangle.Value = true;

                    // 実画像の座標系に変換
                    var viewRect = ClipRectangle(new Rect(MouseDownPoint.Value, draggingVector.Value), ViewImageSize.Value);
                    var imagePixelSize = new Size(MyImage.PixelWidth, MyImage.PixelHeight);
                    var imagePixelRect = ConvertCoordinate(viewRect, ViewImageSize.Value, imagePixelSize);
                    FixedRectangle.Value = imagePixelRect;
                })
                .Repeat()
                .Subscribe(v => draggingVector.Value += v)
                .AddTo(CompositeDisposable);

            // 選択枠のプレビュー
            draggingVector
                .Select(v => ClipRectangle(new Rect(MouseDownPoint.Value, v), ViewImageSize.Value))
                .Subscribe(r => SelectedRectangle.Value = r)
                .AddTo(CompositeDisposable);

            // Viewのサイズ変更に枠サイズを追従
            ViewImageSize
                .Pairwise()
                .Where(x => x.OldItem.Width != 0 && x.OldItem.Height != 0)
                .Select(x => (X: x.NewItem.Width / x.OldItem.Width, Y: x.NewItem.Height / x.OldItem.Height))
                .Subscribe(ratio => SelectedRectangle.Value = MultipleRectangle(SelectedRectangle.Value, ratio.X, ratio.Y))
                .AddTo(CompositeDisposable);
        }

        /// <summary>引数の四角形を指定範囲に制限する</summary>
        private static Rect ClipRectangle(in Rect rect, in Size sizeMax)
        {
            static double Clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));
            static double GetSizeOffset(double value, double min, double max)
            {
                if (value < min) return value - min;
                if (value > max) return value - max;
                return 0;
            }

            var x = Clamp(rect.X, 0, sizeMax.Width - 1);
            var y = Clamp(rect.Y, 0, sizeMax.Height - 1);
            var width = Clamp(rect.Width + GetSizeOffset(rect.X, 0, sizeMax.Width - 1), 1, sizeMax.Width - x);
            var height = Clamp(rect.Height + GetSizeOffset(rect.Y, 0, sizeMax.Height - 1), 1, sizeMax.Height - y);
            return new Rect(x, y, width, height);
        }

        /// <summary>引数の四角形を指定の座標系を正規化する</summary>
        private static Rect ConvertCoordinate(in Rect srcRect, in Size srcSize, in Size destSize)
        {
            static int Clamp(double value, double min, double max) => (int)Math.Round(Math.Max(min, Math.Min(max, value)));

            if (srcSize.Width < 1 || srcSize.Height < 1) throw new ArgumentException(null, nameof(srcSize));
            if (destSize.Width < 1 || destSize.Height < 1) throw new ArgumentException(null, nameof(destSize));

            var x = Clamp(srcRect.X * destSize.Width / srcSize.Width, 0, destSize.Width - 1);
            var y = Clamp(srcRect.Y * destSize.Height / srcSize.Height, 0, destSize.Height - 1);
            var width = Clamp(srcRect.Width * destSize.Width / srcSize.Width, 1, destSize.Width - x);
            var height = Clamp(srcRect.Height * destSize.Height / srcSize.Height, 1, destSize.Height - y);
            return new Rect(x, y, width, height);
        }

        /// <summary>Rectを倍率で伸縮する</summary>
        private static Rect MultipleRectangle(in Rect srcRect, double ratioX, double ratioY)
            => new Rect(srcRect.X * ratioX, srcRect.Y * ratioY, srcRect.Width * ratioX, srcRect.Height * ratioY);
    }
}
