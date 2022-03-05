#nullable disable
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using ReactivePropertySamples.Infrastructures;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input.Manipulations;
using System.Windows.Threading;

namespace ReactivePropertySamples.Views.Pages
{
    public partial class ScheduledNotifierLoggerPage : Controls.MyPageControl
    {
        public ScheduledNotifierLoggerPage()
        {
            InitializeComponent();
            DataContext = new ScheduledNotifierLoggerViewModel();
        }
    }

    class ScheduledNotifierLoggerViewModel : MyDisposableBindableBase
    {
        private readonly MyLogMessageNotifier _logger  = new MyLogMessageNotifier();
        public ReadOnlyReactiveCollection<LogMessage> AllLogs { get; }
        public ReadOnlyReactiveCollection<LogMessage> ErrorLogs { get; }
        public ReactiveCommand OnUIThreadCommand { get; }
        public AsyncReactiveCommand OnAsyncTaskCommand { get; }
        public ReactiveCommand OnDefaultSchedulerCommand { get; }
        public ReactiveCommand ClearErrorLogsCommand { get;  }
        public ScheduledNotifierLoggerViewModel()
        {
            OnUIThreadCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    _logger.Trace("On UIThread");
                    _logger.Warn("On UIThread");
                }, CompositeDisposable.Add)
                .AddTo(CompositeDisposable);

            OnAsyncTaskCommand = new AsyncReactiveCommand()
                .WithSubscribe(async () => await Task.Run(() =>
                {
                    _logger.Debug("On AsyncTask");
                    _logger.Error("On AsyncTask");
                }), CompositeDisposable.Add)
                .AddTo(CompositeDisposable);

            OnDefaultSchedulerCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            OnDefaultSchedulerCommand
                .ObserveOn(Scheduler.Default)
                .Subscribe(x =>
                {
                    _logger.Info("On DefaultScheduler");
                    _logger.Fatal("On AsyncTask");
                })
                .AddTo(CompositeDisposable);

            ClearErrorLogsCommand = new ReactiveCommand().AddTo(CompositeDisposable);

            // 全てのログ
            AllLogs = _logger
                .ToReadOnlyReactiveCollection(onReset: ClearErrorLogsCommand.ToUnit())
                .AddTo(CompositeDisposable);

            // Warn以上のログ (ReactiveCommand.ToUnit() を渡すせば要素をクリアできる）
            ErrorLogs = _logger
                .Where(x => x.Level >= LogLevel.Warn)
                .ToReadOnlyReactiveCollection(onReset: ClearErrorLogsCommand.ToUnit())
                .AddTo(CompositeDisposable);
        }
    }

    // WPFでReactiveProperty入門 ～アプリケーションのステータスやエラー情報をIObservable で通知する
    // https://pierre3.hatenablog.com/entry/2015/11/03/004119
    struct LogLevel : IEquatable<LogLevel>, IComparable<LogLevel>
    {
        public static readonly LogLevel Trace = new LogLevel(nameof(Trace), 0);
        public static readonly LogLevel Debug = new LogLevel(nameof(Debug), 1);
        public static readonly LogLevel Info = new LogLevel(nameof(Info), 2);
        public static readonly LogLevel Warn = new LogLevel(nameof(Warn), 3);
        public static readonly LogLevel Error = new LogLevel(nameof(Error), 4);
        public static readonly LogLevel Fatal = new LogLevel(nameof(Fatal), 5);

        public string Name { get; }
        public int Level { get; }
        public LogLevel(string name, int level) => (Name, Level) = (name, level);

        public static bool operator ==(LogLevel left, LogLevel right) => left.Equals(right);
        public static bool operator !=(LogLevel left, LogLevel right) => !(left == right);
        public static bool operator <(LogLevel left, LogLevel right) => left.CompareTo(right) < 0;
        public static bool operator >(LogLevel left, LogLevel right) => left.CompareTo(right) > 0;
        public static bool operator <=(LogLevel left, LogLevel right) => left.CompareTo(right) <= 0;
        public static bool operator >=(LogLevel left, LogLevel right) => left.CompareTo(right) >= 0;

        public bool Equals(LogLevel other) => Level == other.Level;
        public int CompareTo(LogLevel other) => Level.CompareTo(other.Level);

        public override int GetHashCode() => Name.GetHashCode() ^ Level.GetHashCode();
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj.GetType() != typeof(LogLevel)) return false;
            return Equals((LogLevel)obj);
        }
    }

    class LogMessage
    {
        public DateTime CreatedAt { get; }
        public string Message { get; }
        public LogLevel Level { get; }
        public Exception Exception { get; }
        public bool HasError => Exception is not null;

        private LogMessage(string message, LogLevel level, Exception e) =>
            (CreatedAt, Message, Level, Exception) = (DateTime.Now, message, level, e);

        public override string ToString() => HasError
            ? $"{Level.Name}:[{CreatedAt:G}] {Message} [{Exception.Message}]"
            : $"{Level.Name}:[{CreatedAt:G}] {Message}";

        public static LogMessage CreateTraceLog(string message, Exception e = null) => new LogMessage(message, LogLevel.Trace, e);
        public static LogMessage CreateDebugLog(string message, Exception e = null) => new LogMessage(message, LogLevel.Debug, e);
        public static LogMessage CreateInfoLog(string message, Exception e = null) => new LogMessage(message, LogLevel.Info, e);
        public static LogMessage CreateWarnLog(string message, Exception e = null) => new LogMessage(message, LogLevel.Warn, e);
        public static LogMessage CreateErrorLog(string message, Exception e = null) => new LogMessage(message, LogLevel.Error, e);
        public static LogMessage CreateFatalLog(string message, Exception e = null) => new LogMessage(message, LogLevel.Fatal, e);
    }

    class MyLogMessageNotifier : ScheduledNotifier<LogMessage>, ILogger
    {
        public MyLogMessageNotifier(IScheduler scheduler) : base(scheduler) { }
        public MyLogMessageNotifier() : base() { }

        public void Trace(string message) => Report(LogMessage.CreateTraceLog(message));
        public void Debug(string message) => Report(LogMessage.CreateDebugLog(message));
        public void Info(string message) => Report(LogMessage.CreateInfoLog(message));
        public void Warn(string message) => Report(LogMessage.CreateWarnLog(message));
        public void Error(string message) => Report(LogMessage.CreateErrorLog(message));
        public void Fatal(string message) => Report(LogMessage.CreateFatalLog(message));
        public void Trace(string message, Exception e) => Report(LogMessage.CreateTraceLog(message, e));
        public void Debug(string message, Exception e) => Report(LogMessage.CreateDebugLog(message, e));
        public void Info(string message, Exception e) => Report(LogMessage.CreateInfoLog(message, e));
        public void Warn(string message, Exception e) => Report(LogMessage.CreateWarnLog(message, e));
        public void Error(string message, Exception e) => Report(LogMessage.CreateErrorLog(message, e));
        public void Fatal(string message, Exception e) => Report(LogMessage.CreateFatalLog(message, e));
    }

    interface ILogger
    {
        void Trace(string message);
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);
        void Trace(string message, Exception e);
        void Debug(string message, Exception e);
        void Info(string message, Exception e);
        void Warn(string message, Exception e);
        void Error(string message, Exception e);
        void Fatal(string message, Exception e);
    }
}
