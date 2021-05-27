using Reactive.Bindings;
using Reactive.Bindings.Schedulers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ReactivePropertySamples
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // [ReactiveProperty 7.10.0 をリリースしました](https://zenn.dev/okazuki/articles/62a6c6c9d1a12baccc93)
            // WPFに最適化されたScheduler (デフォルトで問題が起きる Windowの ctor や Loadedイベント時の Validation を改善）
            ReactivePropertyScheduler.SetDefault(new ReactivePropertyWpfScheduler(Dispatcher));
        }

        // ここで例外をハンドルしなければアプリ死にます
        // http://gushwell.ldblog.jp/archives/52335498.html
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var message = $"未ハンドルの例外を検出しましたー" + Environment.NewLine
                + $"{e.Exception.GetType()} : {e.Exception.Message}";

            MessageBox.Show(message, "Exception occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
