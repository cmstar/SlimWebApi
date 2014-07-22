using System;
using System.Reflection;

namespace cmstar.WebApi
{
    /// <summary>
    /// 包含WebAPI注册中的方法的参数有关的信息。
    /// </summary>
    public class ApiParamInfo
    {
        /// <summary>
        /// 初始化<see cref="ApiParamInfo"/>的新实例。
        /// </summary>
        /// <param name="index">参数在方法参数表中的索引。</param>
        /// <param name="parameterInfo"><see cref="ParameterInfo"/>的实例。</param>
        public ApiParamInfo(int index, ParameterInfo parameterInfo)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "The index cannot be less than 0.");

            if (parameterInfo == null)
                throw new ArgumentNullException("parameterInfo");

            Index = index;
            Name = parameterInfo.Name;
            Type = parameterInfo.ParameterType;
            IsGenericCollection = TypeHelper.IsGenericCollection(Type);
        }

        /// <summary>
        /// 获取参数的名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取参数的类型。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 参数的类型是否是一个泛型集合类型。
        /// </summary>
        public bool IsGenericCollection { get; private set; }

        /// <summary>
        /// 获取参数在方法参数表中的索引。
        /// </summary>
        public int Index { get; private set; }
    }
}
