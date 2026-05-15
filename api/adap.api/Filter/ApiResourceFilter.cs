using highspeed.api.Common;
using highspeed.framework.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
namespace highspeed.api.Filter
{
    public class ApiResourceFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        { 
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string ReadBodyAsString(HttpRequest request)
        {
            var body = string.Empty;

            try
            {
                request.EnableBuffering();
                request.Body.Position = 0;
                using (StreamReader sr = new StreamReader(request.Body, leaveOpen: true))
                {
                    body = sr.ReadToEndAsync().Result;
                }
                request.Body.Position = 0;
            }
            catch (Exception ex)
            {
                Logger.Error("ReadBody", ex.Message, ex);
            }

            return body;
        }
    }
}