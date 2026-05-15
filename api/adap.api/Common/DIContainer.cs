using highspeed.framework.Common;

namespace highspeed.api.Common
{
    /// <summary>
    /// 依赖注入容器
    /// </summary>
    public static class DIContainer
    {
        private static IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 配置全局HttpContext
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomHttpContext(this IApplicationBuilder app)
        {
            _httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            return app;
        }

        /// <summary>
        /// 当前请求Http上下文
        /// </summary>
        public static HttpContext CurrentHttpContext => _httpContextAccessor?.HttpContext;

        /// <summary>
        /// 获取DI注入的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? GetSerivce<T>() => CurrentHttpContext == null ? default(T) : (T?)CurrentHttpContext.RequestServices.GetService(typeof(T));

        /// <summary>
        /// 读取当前请求的Body，转为二进制数据
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]?> ReadRequestBodyAsStreamAsync()
        {
            byte[]? result = null;
            try
            {
                if (CurrentHttpContext?.Request == null) return null;

                var request = CurrentHttpContext.Request;
                request.EnableBuffering();
                request.Body.Position = 0;
                using (MemoryStream ms = new MemoryStream())
                {
                    await request.Body.CopyToAsync(ms);
                    result = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("HttpContext.ReadRequestBody", ex.Message, ex);
            }
            return result;
        }
    }
}
