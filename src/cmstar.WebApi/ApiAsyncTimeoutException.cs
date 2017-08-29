using System;
using System.Text;

namespace cmstar.WebApi
{
    /// <summary>
    /// 表示以异步方式调用WebAPI方法时出现的超时。
    /// </summary>
    public class ApiAsyncTimeoutException : Exception
    {
        /// <summary>
        /// 初始化<see cref="ApiAsyncTimeoutException"/>的新实例。
        /// </summary>
        /// <param name="apiMethod">被调用的WebAPI方法。</param>
        /// <param name="timeoutSeconds">超时的时长，单位为秒。</param>
        public ApiAsyncTimeoutException(ApiMethodInfo apiMethod, int timeoutSeconds)
            : base(BuildMessage(apiMethod, timeoutSeconds))
        {
        }

        private static string BuildMessage(ApiMethodInfo apiMethod, int timeoutSeconds)
        {
            // The async invocation of 'TYPE.METHOD(PARAM0,PARAM1,...)' timed out (N seconds)
            var b = new StringBuilder(128);
            b.Append("The async invocation of '");

            // TYPE
            var method = apiMethod.Method;
            var type = method.DeclaringType;
            if (type != null)
            {
                b.Append(type.FullName).Append('.');
            }

            // METHOD
            b.Append(method.Name);

            // PARAM
            b.Append('(');
            var ps = method.GetParameters();
            for (int i = 0; i < ps.Length; i++)
            {
                if (i > 0)
                {
                    b.Append(',');
                }

                b.Append(ps[i].ParameterType.Name);
            }
            b.Append(')');

            // tailing
            b.Append("' timed out (").Append(timeoutSeconds).Append(" seconds).");

            var msg = b.ToString();
            return msg;
        }
    }
}
