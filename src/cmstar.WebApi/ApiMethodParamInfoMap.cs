using System;
using System.Collections.Generic;
using System.Reflection;

namespace cmstar.WebApi
{
    /// <summary>
    /// 包含WebAPI方法的参数信息和有关操作。
    /// </summary>
    public class ApiMethodParamInfoMap
    {
        private readonly Dictionary<string, ApiParamInfo> _paramTypeMap;

        /// <summary>
        /// 初始化<see cref="ApiMethodParamInfoMap"/>的新实例。
        /// </summary>
        /// <param name="methodInfo">注册的方法。</param>
        /// <param name="paramNameComparer">
        /// 指定如何比较参数的名称。若设置为null，则使用<see cref="StringComparer.OrdinalIgnoreCase"/>。
        /// </param>
        public ApiMethodParamInfoMap(MethodInfo methodInfo, IEqualityComparer<string> paramNameComparer = null)
        {
            ArgAssert.NotNull(methodInfo, "methodInfo");
            Method = methodInfo;

            var param = methodInfo.GetParameters();
            var comparer = paramNameComparer ?? StringComparer.OrdinalIgnoreCase;
            _paramTypeMap = new Dictionary<string, ApiParamInfo>(param.Length, comparer);

            for (var i = 0; i < param.Length; i++)
            {
                var p = param[i];
                var paramName = p.Name;
                var paramInfo = new ApiParamInfo(i, p);
                _paramTypeMap.Add(paramName, paramInfo);
            }
        }

        /// <summary>
        /// 获取当前实例所关联的<see cref="MethodInfo"/>。
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// 获取参数名称与<see cref="ApiParamInfo"/>的序列。
        /// </summary>
        public IEnumerable<KeyValuePair<string, ApiParamInfo>> ParamInfos
        {
            get { return _paramTypeMap; }
        }

        /// <summary>
        /// 获取方法中参数的数量。
        /// </summary>
        public int ParamCount
        {
            get { return _paramTypeMap.Count; }
        }

        /// <summary>
        /// 获取具有指定名称的<see cref="ApiParamInfo"/>。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="paramInfo">
        /// 输出参数。当方法返回<c>true</c>时为具有指定名称的<see cref="ApiParamInfo"/>的实例。
        /// </param>
        /// <returns><c>true</c>当指定的名称存在；否则为<c>false</c>。</returns>
        public bool TryGetParamInfo(string name, out ApiParamInfo paramInfo)
        {
            return _paramTypeMap.TryGetValue(name, out paramInfo);
        }
    }
}
