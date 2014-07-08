using System;
using System.Reflection;

namespace cmstar.WebApi
{
    public class ApiParamInfo
    {
        public ApiParamInfo(int index, ParameterInfo parameterInfo)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "The index cannot be less than 0.");

            if (parameterInfo == null)
                throw new ArgumentNullException("parameterInfo");

            Index = index;
            Name = parameterInfo.Name;
            Type = parameterInfo.ParameterType;
        }

        public string Name { get; private set; }
        public Type Type { get; private set; }
        public int Index { get; private set; }
    }
}
