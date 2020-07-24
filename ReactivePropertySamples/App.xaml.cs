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
