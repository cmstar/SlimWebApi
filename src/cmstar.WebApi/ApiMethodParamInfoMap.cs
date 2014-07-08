using System;
using System.Collections.Generic;
using System.Reflection;

namespace cmstar.WebApi
{
    public class ApiMethodParamInfoMap
    {
        private readonly Dictionary<string, ApiParamInfo> _paramTypeMap;

        public ApiMethodParamInfoMap(MethodInfo methodInfo, IEqualityComparer<string> methodNameComparer = null)
        {
            ArgAssert.NotNull(methodInfo, "methodInfo");
            Method = methodInfo;

            var param = methodInfo.GetParameters();
            var comparer = methodNameComparer ?? ApiEnvironment.DefaultMethodNameComparer;
            _paramTypeMap = new Dictionary<string, ApiParamInfo>(param.Length, comparer);

            for (var i = 0; i < param.Length; i++)
            {
                var p = param[i];
                var paramName = p.Name;
                var paramType = p.ParameterType;

                if (paramType.IsSubclassOf(typeof(IConvertible)))
                {
                    var msg = string.Format(
                        "The parameter \"{0}\" (type {1}) of method {2} cannot be convert from the query string.",
                        paramName, paramType, methodInfo.Name);
                    throw new ArgumentException(msg, "methodInfo");
                }

                var paramInfo = new ApiParamInfo(i, p);
                _paramTypeMap.Add(paramName, paramInfo);
            }
        }

        public MethodInfo Method { get; private set; }

        public IEnumerable<KeyValuePair<string, ApiParamInfo>> ParamInfos
        {
            get { return _paramTypeMap; }
        }

        public int ParamCount
        {
            get { return _paramTypeMap.Count; }
        }

        public bool TryGetParamInfo(string name, out ApiParamInfo paramInfo)
        {
            return _paramTypeMap.TryGetValue(name, out paramInfo);
        }
    }
}
