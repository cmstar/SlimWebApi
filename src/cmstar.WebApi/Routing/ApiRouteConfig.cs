using System.Web.Routing;

namespace cmstar.WebApi.Routing
{
    /// <summary>
    /// ����WebAPI·��ע������Ļ�����Ϣ��
    /// </summary>
    public class ApiRouteConfig
    {
        /// <summary>
        /// URL��
        /// </summary>
        public string Url;

        /// <summary>
        /// ��δ��URL�и�����ز���ʱ��������ʹ�õ�Ĭ��ֵ��
        /// </summary>
        public RouteValueDictionary Defaults;

        /// <summary>
        /// ��������ƥ���������������ʽ��
        /// </summary>
        public RouteValueDictionary Constraints;
    }
}