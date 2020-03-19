using System.Collections.Generic;
using System.Text;

namespace cmstar.WebApi
{
    /// <summary>
    /// 表示一个日志事件的消息部分。消息由一组 key-value 构成，且 key 在消息中是唯一的。
    /// </summary>
    public class LogMessage
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        private static readonly SingleEntryDictionary<string, string> NoProperty
            = new SingleEntryDictionary<string, string>(readOnly: true);

        private Dictionary<string, string> _properties;
        private string _renderedMessage;
        private string _message;

        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        public LogMessage()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        /// <param name="message">消息的文本。</param>
        public LogMessage(string message)
        {
            Message = message;
        }

        /// <summary>
        /// 获取或设置消息的文本。没有设置时为空字符串，不会为null。
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value ?? string.Empty; }
        }

        /// <summary>
        /// 表示当前的消息是否有附加属性。
        /// </summary>
        public bool HasProperties => _properties != null;

        /// <summary>
        /// 设置消息的附加属性。
        /// </summary>
        /// <param name="name">属性的名称。</param>
        /// <param name="value">属性的值。</param>
        public void SetProperty(string name, string value)
        {
            ArgAssert.NotNull(name, "name");
            ArgAssert.NotNull(value, "value");

            if (_properties == null)
            {
                _properties = new Dictionary<string, string>();
            }

            _properties[name] = value;
        }

        /// <summary>
        /// 获取具有指定名称的消息附加属性。若具有指定名称的属性不存在，返回null。
        /// </summary>
        /// <param name="name">属性的名称。</param>
        /// <returns>属性的值。若具有指定名称的属性不存在，返回null。</returns>
        public string GetProperty(string name)
        {
            ArgAssert.NotNull(name, "name");

            string value;
            return _properties != null && _properties.TryGetValue(name, out value)
                ? value : null;
        }

        /// <summary>
        /// 获取消息的所有附加属性。
        /// </summary>
        /// <returns>一个序列，用于遍历消息的所有附加属性。</returns>
        public IEnumerable<KeyValuePair<string, string>> GetProperies()
        {
            if (_properties == null)
                return NoProperty;

            return _properties;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (_properties == null)
                return Message;

            if (_renderedMessage != null)
                return _renderedMessage;

            var sb = new StringBuilder();
            var first = true;

            if (!string.IsNullOrEmpty(Message))
            {
                sb.Append(Message);
                first = false;
            }

            foreach (var p in _properties)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.AppendLine();
                }

                sb.Append(p.Key).Append(": ").Append(p.Value);
            }

            _renderedMessage = sb.ToString();
            return _renderedMessage;
        }
    }
}
