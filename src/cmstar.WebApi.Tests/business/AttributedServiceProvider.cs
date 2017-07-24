using System;
using Common.Logging;

namespace cmstar.WebApi
{
    public class AttributedServiceProvider
    {
        private int _value;

        // ���API��������û�п�������
        [ApiMethod]
        public int Zero()
        {
            // ����ͨ�� ApiMethodContext.Current.Raw ֱ�ӷ��ʵ�ǰ����� HttpContext��
            // ���գ�Ҳ����ֱ��ͨ�� HttpContext.Current��
            ApiMethodContext.Current.Raw.Response.Headers.Add(
                "Custom-Head",
                $"raw {ApiMethodContext.Current.Raw != null}");

            return 0;
        }

        // ����ָ�������ɹ�ִ��ʱ�����־����־����
        [ApiMethod(SuccessLogLevel = LogLevel.Fatal)]
        public void FatalLog() { }

        [ApiMethod(SuccessLogLevel = LogLevel.Trace)]
        public void TraceLog() { }

        // �����������û��泬ʱ�������Զ�����
        [ApiMethod(AutoCacheEnabled = true, CacheExpiration = 3)]
        public DateTime Now()
        {
            return DateTime.Now;
        }

        // ʹ��ApiMethodContext�ֹ������棬��Ӧ������ĳ���������API��ͬʱ�ж�д��
        [ApiMethod(CacheExpiration = 5)]
        public int ManualCache()
        {
            // �˷���ʹ��ApiMethodContext�ֹ�������
            var value = ApiMethodContext.Current.GetCachedResult();
            if (value != null)
                return (int)value;

            _value++;
            ApiMethodContext.Current.SetCachedResult(_value);
            return _value;
        }

        [ApiMethod(CompressionMethods = ApiCompressionMethods.GZip)]
        public string ForceGzipString()
        {
            return Guid.NewGuid().ToString();
        }

        [ApiMethod(CompressionMethods = ApiCompressionMethods.Defalte)]
        public string ForceDeflateString()
        {
            return Guid.NewGuid().ToString();
        }

        [ApiMethod(CompressionMethods = ApiCompressionMethods.Auto)]
        public string AutoCompressionString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}