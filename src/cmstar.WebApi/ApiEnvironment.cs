﻿using System;
using System.Net;
using System.Web;
using System.Web.Configuration;

namespace cmstar.WebApi
{
    /// <summary>
    /// 包含WebAPI调用上下文所需的一些属性与方法。
    /// </summary>
    public static class ApiEnvironment
    {
        // 表示一个未初始化的值。
        private const int Uninitialized = -1;

        // // 存储配置中 system.web/httpRuntime 元素的 executetionTimeout 值，单位为秒。
        private static int _executionTimeout = Uninitialized;

        // 异步的WebAPI方法的超时时间，单位为秒。
        private static int _asyncTimeout = Uninitialized;

        /// <summary>
        /// 用于<see cref="ApiResponse{T}.Code"/>，表示客户端请求不能被识别。
        /// 值同<see cref="HttpStatusCode.BadRequest"/>（400）。
        /// </summary>
        public static int CodeBadRequest { get; } = (int)HttpStatusCode.BadRequest;

        /// <summary>
        /// 用于<see cref="ApiResponse{T}.Code"/>，表示服务端内部异常。
        /// 值同<see cref="HttpStatusCode.InternalServerError"/>（500）。
        /// </summary>
        public static int CodeInternalError { get; } = (int)HttpStatusCode.InternalServerError;

        /// <summary>
        /// 获取默认的HTTP请求执行超时。
        /// 即配置文件中 system.web/httpRuntime 元素的 executetionTimeout 属性的值。
        /// </summary>
        public static int DefaultExecutionTimeout
        {
            get
            {
                if (_executionTimeout != Uninitialized)
                    return _executionTimeout;

                // 看起来 ASP.net 并没有提供公共API能在静态上下文理获取 executetionTimeout 值，
                // 这里得自己解析一下配置文件。
                var section = WebConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;

                // 若没有拿到对应的配置，创建一个，新实例的属性具有默认的配置值。
                if (section == null)
                {
                    section = new HttpRuntimeSection();
                }

                _executionTimeout = (int)section.ExecutionTimeout.TotalSeconds;
                return _executionTimeout;
            }
        }

        /// <summary>
        /// 获取或设置异步的WebAPI方法的超时时间，单位为秒。若WebAPI方法没有单独指定超时时间，则使用此超时设置。
        /// 初始值同<see cref="DefaultExecutionTimeout"/>；若设置为0，则没有超时限制。
        /// </summary>
        public static int AsyncTimeout
        {
            get
            {
                if (_asyncTimeout == Uninitialized)
                {
                    _asyncTimeout = DefaultExecutionTimeout;
                }

                return _asyncTimeout;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "The value must be equal to or greater than zero.");

                _asyncTimeout = value;
            }
        }
    }
}
