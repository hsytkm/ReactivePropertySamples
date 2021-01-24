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
        public IReadOnlyReactiveProperty<IReadOnlyCollection<Point>> PreviewRectanglePoints { get; }
        public IReactiveProperty<bool> IsFinishedSelectingRectangle  { get; }
        public IReactiveProperty<Size> ViewImageSize { get; }
        public IReactiveProperty<Rect> SelectedRectangle { get; }

        public SelectRectangleViewModel()
        {
            MouseDownPoint = new ReactiveProperty<Point>(mode: ReactivePropertyMode.None).AddTo(CompositeDisposable);
            MouseUpUnit = new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None).AddTo(CompositeDisposable);
            MouseMovePoint = new ReactiveProperty<Point>().AddTo(CompositeDisposable);
            IsFinishedSelectingRectangle = new ReactiveProperty<bool>().AddTo(CompositeDisposable);
            ViewImageSize = new ReactiveProperty<Size>().AddTo(CompositeDisposable);
            SelectedRectangle = new ReactiveProperty<Rect>().AddTo(CompositeDisposable);

            var draggingVector = new ReactiveProperty<Vector>().AddTo(CompositeDisposable);

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
                    var viewRect = new Rect(MouseDownPoint.Value, draggingVector.Value);
                    var imagePixelSize = new Size(MyImage.PixelWidth, MyImage.PixelHeight);
                    SelectedRectangle.Value = ConvertCoordinate(viewRect, ViewImageSize.Value, imagePixelSize);
                })
                .Repeat()
                .Subscribe(v => draggingVector.Value += v)
                .AddTo(CompositeDisposable);

            // Viewの選択範囲プレビュー
            PreviewRectanglePoints = draggingVector
                .Select(v => ToConerPoints(MouseDownPoint.Value, v).ToArray())
                .ToReadOnlyReactiveProperty()
                .AddTo(CompositeDisposable);
        }

        private static IEnumerable<Point> ToConerPoints(Point leftTop, Vector vector)
        {
            if (vector == default) yield break;

            yield return leftTop;
            yield return new Point(leftTop.X + vector.X, leftTop.Y);
            yield return new Point(leftTop.X + vector.X, leftTop.Y + vector.Y);
            yield return new Point(leftTop.X, leftTop.Y + vector.Y);
        }

        private static Rect ConvertCoordinate(Rect srcRect, Size srcSize, Size destSize)
        {
            if (srcSize.Width == 0 || srcSize.Height == 0) throw new ArgumentException();
            if (destSize.Width == 0 || destSize.Height == 0) throw new ArgumentException();

            var x = Math.Round(srcRect.X * destSize.Width / srcSize.Width);
            var y = Math.Round(srcRect.Y * destSize.Height / srcSize.Height);
            var width = Math.Round(srcRect.Width * destSize.Width / srcSize.Width);
            var height = Math.Round(srcRect.Height * destSize.Height / srcSize.Height);
            return new Rect(x, y, width, height);
        }

    }
}
