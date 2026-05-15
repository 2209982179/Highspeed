using highspeed.api.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
namespace highspeed.api.Filter
{
    public class ApiResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result == null)
            {
                return;
            }

            // Ok()
            if (context.Result is OkResult okResult)
            {
                context.Result = new ObjectResult(new ApiResult
                {
                    Code = okResult.StatusCode,
                    Success = true,
                    Data = string.Empty,
                    Message = string.Empty
                });
            }
            else if (context.Result is ActionResponse actionResponse)
            {
                context.Result = new ObjectResult(new ApiResult
                {
                    Code = 200,
                    Success = true,
                    Data = actionResponse.Guid?.ToString(),
                    Message = string.Empty,
                    ActionGuid = actionResponse.Guid,
                });
            }
            // Ok(...) / Problem(...) / ...
            else if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.DeclaredType is null)
                {
                    string message = "";
                    // ValidationProblem(...)
                    if (objectResult.Value is ValidationProblemDetails validationProblemDetails && validationProblemDetails.Errors.Any())
                    {
                        message = string.Join(";", validationProblemDetails.Errors.SelectMany(x => x.Value));
                        //message = validationProblemDetails.Errors.First().Value.First();
                    }
                    // Problem(...)
                    else if (objectResult.Value is ProblemDetails problemDetails)
                    {
                        message = problemDetails.Title;
                    }
                    else
                    {
                    }

                    context.Result = new ObjectResult(new ApiResult
                    {
                        Code = objectResult.StatusCode,
                        Success = !(objectResult.Value is ProblemDetails)
                                  && objectResult.StatusCode.HasValue
                                  && objectResult.StatusCode == 200,
                        Data = objectResult.Value,
                        Message = message
                    });
                }
                else
                {
                    context.Result = new ObjectResult(new ApiResult
                    {
                        Code = 200,
                        Success = true,
                        Data = objectResult.Value,
                        Message = string.Empty
                    });
                }
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new ApiResult
                {
                    Code = 200,
                    Success = true,
                    Data = string.Empty,
                    Message = string.Empty
                });
            }
            else if (context.Result is FileStreamResult || context.Result is FileContentResult)
            {
                var result = context.Result as FileResult;
                context.HttpContext.Response.Headers.Add(Consts.HeaderKey_FileName, WebUtility.UrlEncode(result.FileDownloadName));
                context.HttpContext.Response.ContentType = result.ContentType;
            }
            else
            {
                throw new Exception($"Unhandled result type：{context.Result.GetType().Name}");
            }
        }
    }
}