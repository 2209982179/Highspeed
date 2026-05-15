using highspeed.framework.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace highspeed.api.Filter
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            try
            {
                var session = GlobalContext.CurrentSession;
                Logger.Error($"UserUid:{session.UserId} UserName:{session.UserName}", "");
                Logger.Error("", context.Exception.Message, context.Exception);
            }
            catch { }

            context.Result = new ObjectResult(new ApiResult
            {
                Code = 501,
                Success = false,
                Data = "Internal server error",
                Message = GetMessage(context.Exception)
            });
        }

        private string GetMessage(Exception ex)
        {
            if (ex.InnerException == null) return ex.Message;
            else return GetMessage(ex.InnerException);
        }
    }
}