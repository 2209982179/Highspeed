using highspeed.api.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
namespace highspeed.api.Filter
{
    public class ApiActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            #region 刷新Token 
            var old = context.HttpContext.Request.Headers[JwtHelper.HeaderKey].ToString();
            var jwt = DIContainer.GetSerivce<JwtHelper>();
            // 设置允许跨域转递的Header
            context.HttpContext.Response.Headers.Add(
                "Access-Control-Expose-Headers",
                new StringValues(
                    new string[] {
                        Consts.HeaderKey_Authorization,
                        Consts.HeaderKey_PermissionCheck,
                        Consts.HeaderKey_Readonly,
                        Consts.HeaderKey_FileName,
            }));
            context.HttpContext.Response.Headers.Add(JwtHelper.HeaderKey, jwt?.RefreshToken(old));
            #endregion

            context.HttpContext.Response.Headers.Add(Consts.HeaderKey_PermissionCheck, context.HttpContext.Request.Headers[Consts.HeaderKey_PermissionCheck]);
            #region Readonly
            if (context.HttpContext.Request.Headers.TryGetValue(Consts.HeaderKey_PermissionCheck, out StringValues result)
                && CheckPermissionResult.Readonly.ToString().Equals(result.First()))
                context.HttpContext.Response.Headers.Add(Consts.HeaderKey_Readonly, "true");
            #endregion
        }

        public async void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}