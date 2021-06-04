using System;
using System.Reactive.Disposables;
using System.Windows.Controls;

namespace ReactivePropertySamples.Views.Controls
{
    public abstract class DisposableMyControl : UserControl, IDisposable
    {
        protected readonly CompositeDisposable CompositeDisposable = new();

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            //Debug.WriteLine($"DisposableUserControl: {this}");

            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    CompositeDisposable.Dispose();

                    if (DataContext is IDisposable vmodel)
                    {
                        vmodel.Dispose();
                    }

                    // 全ての子コントロールをDispose(◆孫が何度もDisposeされて気になる…)
                    this.DisposeDescendants();
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。
                DataContext = null; // メモリリークしてる気がするので明示的に(気のせいかも…)

                disposedValue = true;
            }
            //Debug.WriteLine($"End DisposableUserControl: {this}");
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~DisposableMyControl()
        // {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
