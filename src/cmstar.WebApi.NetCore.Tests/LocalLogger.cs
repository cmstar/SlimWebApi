using System;
using System.Text;
using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.Factory;
using Common.Logging.Simple;

namespace cmstar.WebApi
{
    public class LocalLoggerFactory : AbstractSimpleLoggerFactoryAdapter
    {
        public LocalLoggerFactory()
            : base(new NameValueCollection())
        {
            Console.WriteLine(1);
        }

        public LocalLoggerFactory(NameValueCollection properties)
            : base(properties)
        {
        }

        public LocalLoggerFactory(LogLevel level, bool showDateTime, bool showLogName, bool showLevel, string dateTimeFormat)
            : base(level, showDateTime, showLogName, showLevel, dateTimeFormat)
        {
        }

        protected override ILog CreateLogger(
            string name, LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
        {
            return new LocalLogger();
        }
    }

    public class LocalLogger : AbstractLogger
    {
        private LogLevel _logLevel = LogLevel.All;

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
            if (!IsLogLevelEnabled(level))
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
            Console.WriteLine(body);
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
            if (ex is AggregateException)
                return ex.ToString();

            var sb = new StringBuilder(0xff);
            for (var inner = ex; inner != null; inner = inner.InnerException)
            {
                if (inner != ex)
                    sb.Append(Environment.NewLine).Append("===>");

                if (inner is AggregateException)
                {
                    sb.Append(inner);
                    break;
                }

                sb.Append(inner.GetType().FullName).Append(": ");
                sb.Append(inner.Message).Append(Environment.NewLine);
                sb.Append("StackTrace:");
                sb.Append(inner.StackTrace);
            }
            return sb.ToString();
        }
    }
}