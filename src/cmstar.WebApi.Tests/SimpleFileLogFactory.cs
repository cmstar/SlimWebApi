using System;
using System.Configuration;
using System.IO;
using System.Text;
using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.Factory;
using Common.Logging.Simple;

namespace cmstar.WebApi
{
    public class SimpleFileLogFactory : AbstractSimpleLoggerFactoryAdapter
    {
        public SimpleFileLogFactory(NameValueCollection properties)
            : base(properties)
        {
        }

        public SimpleFileLogFactory(LogLevel level, bool showDateTime, bool showLogName, bool showLevel, string dateTimeFormat)
            : base(level, showDateTime, showLogName, showLevel, dateTimeFormat)
        {
        }

        protected override ILog CreateLogger(
            string name, LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
        {
            return new SimpleFileLog(level);
        }
    }

    public class SimpleFileLog : AbstractLogger
    {
        private static readonly object GlobalSyncBlock = new object();
        private const LogLevel DefaultLogLevel = LogLevel.Info;

        private readonly object _taskSyncBlock = new object();
        private readonly string _logPathRoot;
        private readonly bool _canWriteLog;
        private LogLevel _logLevel;

        private static string GetLogPathFromAppSetting()
        {
            var config = ConfigurationManager.AppSettings["simplefilelog.path"];
            if (string.IsNullOrWhiteSpace(config))
                return null;

            var path = ResolvePath(config);
            var tail = path[path.Length - 1];
            if (tail != '/' || tail != '\\')
            {
                path += '/';
            }

            return path;
        }

        private static LogLevel GetLogLevelFromAppSetting()
        {
            var config = ConfigurationManager.AppSettings["simplefilelog.loglevel"];
            if (config == null)
                return DefaultLogLevel;

            config = config.ToLower();
            switch (config)
            {
                case "trace": return LogLevel.Trace;
                case "debug": return LogLevel.Debug;
                case "info": return LogLevel.Info;
                case "warn": return LogLevel.Warn;
                case "error": return LogLevel.Error;
                case "fatal": return LogLevel.Fatal;
                default: return DefaultLogLevel;
            }
        }

        public SimpleFileLog()
            : this(GetLogLevelFromAppSetting(), GetLogPathFromAppSetting())
        {
        }

        public SimpleFileLog(LogLevel logLevel)
            : this(logLevel, GetLogPathFromAppSetting())
        {
        }

        public SimpleFileLog(LogLevel logLevel, string logPath)
        {
            _logLevel = logLevel;
            _logPathRoot = logPath;
            _canWriteLog = _logPathRoot != null;
        }

        public LogLevel LogLevel
        {
            get { return _logLevel; }
            set { _logLevel = value; }
        }

        public override bool IsTraceEnabled => IsLogLevelEnabled(LogLevel.Trace);

        public override bool IsDebugEnabled => IsLogLevelEnabled(LogLevel.Debug);

        public override bool IsErrorEnabled => IsLogLevelEnabled(LogLevel.Error);

        public override bool IsFatalEnabled => IsLogLevelEnabled(LogLevel.Fatal);

        public override bool IsInfoEnabled => IsLogLevelEnabled(LogLevel.Info);

        public override bool IsWarnEnabled => IsLogLevelEnabled(LogLevel.Warn);

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            if (!_canWriteLog || !IsLogLevelEnabled(level))
                return;

            var msgBuilder = new StringBuilder(128);
            msgBuilder.AppendFormat("#{0:MMdd HHmmss} {1}# ", DateTime.Now, level);
            msgBuilder.Append(message);

            if (exception != null)
            {
                var exDes = GetFullDescrption(exception);
                msgBuilder.AppendLine();
                msgBuilder.Append(exDes);
            }

            var body = msgBuilder.ToString();
            var file = string.Concat(_logPathRoot, DateTime.Now.ToString("yyyyMMdd"), ".txt");

            if (!Directory.Exists(_logPathRoot))
            {
                lock (GlobalSyncBlock)
                {
                    if (!Directory.Exists(_logPathRoot))
                    {
                        Directory.CreateDirectory(_logPathRoot);
                    }
                }
            }

            lock (_taskSyncBlock)
            {
                using (var sw = new StreamWriter(file, true, Encoding.UTF8))
                {
                    sw.WriteLine(body);
                }

                Console.WriteLine(body);
            }
        }

        private bool IsLogLevelEnabled(LogLevel logLevel)
        {
            if (_logLevel == LogLevel.Off)
                return false;

            if (_logLevel == LogLevel.All)
                return true;

            var enabled = ((int)_logLevel) <= ((int)logLevel);
            return enabled;
        }

        private static string GetFullDescrption(Exception ex)
        {
#if !NET35
            if (ex is AggregateException)
                return ex.ToString();
#endif

            var sb = new StringBuilder(0xff);
            for (var inner = ex; inner != null; inner = inner.InnerException)
            {
                if (inner != ex)
                    sb.Append(Environment.NewLine).Append("===>");

#if !NET35
                if (inner is AggregateException)
                {
                    sb.Append(inner);
                    break;
                }
#endif

                sb.Append(inner.GetType().FullName).Append(": ");
                sb.Append(inner.Message).Append(Environment.NewLine);
                sb.Append("StackTrace:");
                sb.Append(inner.StackTrace);
            }
            return sb.ToString();
        }

        private const string AppDomainPathPlaceHolder = "~"; //表示当前程序域根目录的符号
        private const string AssemblyPathPlaceHolder = "!"; //表示当前程序集目录的符号

        private static string ResolvePath(string path)
        {
            var domainDir = AppDomain.CurrentDomain.BaseDirectory;

            if (string.IsNullOrEmpty(path))
                return domainDir;

            if (path.StartsWith(AppDomainPathPlaceHolder))
            {
                path = domainDir + path.Substring(AppDomainPathPlaceHolder.Length);
            }
            else if (path.StartsWith(AssemblyPathPlaceHolder))
            {
                var assemblyPath = CurrentAssemblyDirectory();
                path = assemblyPath + path.Substring(AssemblyPathPlaceHolder.Length);
            }

            //处理包含绝对路径但开头为斜杠的情况 /C:/abc \c:\abc，将开头的斜杠移除
            if (path.Length >= 3 && path[2] == ':' && (path[0] == '\\' || path[0] == '/'))
                path = path.Substring(1);

            try
            {
                return Path.GetFullPath(path);
            }
            catch (NotSupportedException ex)
            {
                var msg = $"路径格式不被支持：{path}{Environment.NewLine}{ex.Message}";
                throw new NotSupportedException(msg, ex);
            }
        }

        private static string CurrentAssemblyDirectory()
        {
            //对于web app，其config文件为 web.config，这里就使用配置文件名称来区分是否是web app，
            //而不是使用System.Web中的有关方法，以避免引用System.Web程序集。
            //尚未发现此处理方式潜在的问题。
            var cfgFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            var path = AppDomain.CurrentDomain.BaseDirectory;

            if (cfgFile.ToLower().EndsWith("web.config"))
            {
                path += "/bin";
            }

            return ResolvePath(path);
        }
    }
}