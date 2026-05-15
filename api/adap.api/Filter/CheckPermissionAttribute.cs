using highspeed.api.Common;
using highspeed.framework;
using Microsoft.AspNetCore.Mvc.Filters;
namespace highspeed.api.Filter
{
    /// <summary>
    /// 鉴权结果
    /// </summary>
    public enum CheckPermissionResult
    {
        /// <summary>
        /// 跳过鉴权
        /// </summary>
        Skip,
        /// <summary>
        /// 忽略鉴权结果
        /// </summary>
        Ignore,
        /// <summary>
        /// 鉴权成功
        /// </summary>
        Success,
        /// <summary>
        /// 鉴权为只读
        /// </summary>
        Readonly,
        /// <summary>
        /// 鉴权异常
        /// </summary>
        Error,
    }

    /// <summary>
    /// 鉴权对象
    /// </summary>
    public enum CheckPermissionTarget : uint
    {
        Package = 1,
        Diagram = 1 << 1,
        Element = 1 << 2,
        Connector = 1 << 3,
        All = Package | Diagram | Element | Connector
    }

    /// <summary>
    /// 鉴权接口
    /// </summary>
    public interface IPermissionChecker
    {
        CheckPermissionResult Check(object context);
    }

    /// <summary>
    /// 鉴权Attribute
    /// </summary>
    public abstract class CheckPermissionAttribute : Attribute, IAsyncActionFilter, IPermissionChecker
    {
        protected IPermissionChecker Checker;
        public CheckPermissionAttribute() { }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            CheckPermissionResult result = CheckPermissionResult.Success;
            if (Checker != null)
            {
                result = Checker.Check(context);
            }

            string key = Consts.HeaderKey_PermissionCheck;
            if (context.HttpContext.Request.Headers.ContainsKey(key))
            {
                var curr = Enum.Parse(typeof(CheckPermissionResult), context.HttpContext.Request.Headers[key].ToString());
                if ((int)result > (int)curr) // 多次鉴权时以最高级别为最终结果
                {
                    context.HttpContext.Request.Headers[key] = result.ToString();
                }
            }
            else context.HttpContext.Request.Headers.Add(key, result.ToString());
            //Logger.Debug($"CheckPermission - {this.GetType().Name}", $"Current result: {context.HttpContext.Request.Headers[key]}");

            switch (result)
            {
                case CheckPermissionResult.Error:
                    throw new Exception("当前用户没有执行此操作的权限!");
                default:
                    break;
            }

            await next();
        }

        public abstract CheckPermissionResult Check(object context);

        protected bool CanWrite(CheckPermissionTarget CheckTarget, params int[] id)
        {  
            return true;
        }

        protected bool CanWrite(CheckPermissionTarget CheckTarget, params string[] guid)
        {
            return true;
        }
    }

    /// <summary>
    /// 根据Header中数据鉴权
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionHeaderChecker : CheckPermissionAttribute
    {
        private CheckPermissionTarget CheckTarget;
        private string QueryFilter;
        private CheckPermissionResult OnFailed;
        public PermissionHeaderChecker(CheckPermissionTarget target = CheckPermissionTarget.All, string queryFilter = null, CheckPermissionResult onFailed = CheckPermissionResult.Error)
        {
            Checker = this;
            CheckTarget = target;
            QueryFilter = queryFilter;
            OnFailed = onFailed;
        }

        public override CheckPermissionResult Check(object context)
        {
            var request = (context as ActionExecutingContext).HttpContext.Request;

            if (!string.IsNullOrWhiteSpace(QueryFilter)
                   && request.QueryString.Value?.Contains(QueryFilter) != true)
                return CheckPermissionResult.Skip;

            var ids = request.Headers["SelectedIds"].ToString();
            if (!string.IsNullOrEmpty(ids))
            {
                var selectedIds = ids.JsonToObject<SelectedIds>();
                var canWrite = true;
                if (CheckTarget.HasFlag(CheckPermissionTarget.Package))
                {
                    //Logger.Debug("PermissionHeaderChecker - Package", string.Join(",", selectedIds.PackageIds));
                    if (selectedIds.PackageIds?.Count() > 0)
                        canWrite = CanWrite(CheckTarget, selectedIds.PackageIds.ToArray());
                }
                if (canWrite && CheckTarget.HasFlag(CheckPermissionTarget.Diagram))
                {
                    //Logger.Debug("PermissionHeaderChecker - Diagram", string.Join(",", selectedIds.DiagramIds));
                    if (selectedIds.DiagramIds?.Count() > 0)
                        canWrite = CanWrite(CheckTarget, selectedIds.DiagramIds.ToArray());
                }
                if (canWrite && CheckTarget.HasFlag(CheckPermissionTarget.Element))
                {
                    //Logger.Debug("PermissionHeaderChecker - Element", string.Join(",", selectedIds.ElementIds));
                    if (selectedIds.ElementIds?.Count() > 0)
                        canWrite = CanWrite(CheckTarget, selectedIds.ElementIds.ToArray());
                }
                if (canWrite && CheckTarget.HasFlag(CheckPermissionTarget.Connector))
                {
                    //Logger.Debug("PermissionHeaderChecker - Connector", string.Join(",", selectedIds.ConnectorIds));
                    if (selectedIds.ConnectorIds?.Count() > 0)
                        canWrite = CanWrite(CheckTarget, selectedIds.ConnectorIds.ToArray());
                }
                return canWrite ? CheckPermissionResult.Success : OnFailed;
            }
            return CheckPermissionResult.Skip;
        }

        public class SelectedIds
        {
            public List<int> PackageIds { get; set; }
            public List<int> DiagramIds { get; set; }
            public List<int> ElementIds { get; set; }
            public List<int> ConnectorIds { get; set; }
        }
    }

    /// <summary>
    /// 根据Query参数鉴权
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionQueryChecker : CheckPermissionAttribute
    {
        private string Param;
        private string QueryFilter;
        private CheckPermissionTarget CheckTarget;
        private CheckPermissionResult OnFailed;

        public PermissionQueryChecker() { }

        public PermissionQueryChecker(string param, CheckPermissionTarget target, string queryFilter = null, CheckPermissionResult onFailed = CheckPermissionResult.Error)
        {
            Checker = this;
            Param = param;
            QueryFilter = queryFilter;
            CheckTarget = target;
            OnFailed = onFailed;
        }

        public override CheckPermissionResult Check(object context)
        {
            if (!string.IsNullOrWhiteSpace(Param))
            {
                var request = (context as ActionExecutingContext).HttpContext.Request;

                if (!string.IsNullOrWhiteSpace(QueryFilter)
                   && request.QueryString.Value?.Contains(QueryFilter) != true)
                    return CheckPermissionResult.Skip;

                var query = request.Query;
                var val = query.FirstOrDefault(r => r.Key == Param).Value.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(val)) return CheckPermissionResult.Skip;

                int.TryParse(val, out int id);
                var canWrite = id > 0
                                ? CanWrite(CheckTarget, id) // id
                                : CanWrite(CheckTarget, val); // guid

                return canWrite ? CheckPermissionResult.Success : OnFailed;
            }
            return CheckPermissionResult.Skip;
        }
    }

    /// <summary>
    /// 自定义鉴权
    /// </summary>
    public class PermissionCustomChecker : CheckPermissionAttribute
    {
        protected IPermissionChecker CustomChecker;
        public PermissionCustomChecker(IPermissionChecker checker) { Checker = this; CustomChecker = checker; }

        protected Type CustomCheckerType;
        public PermissionCustomChecker(Type checkerType) { Checker = this; CustomCheckerType = checkerType; }

        public override CheckPermissionResult Check(object context)
        {
            if (CustomChecker != null)
            {
                var request = (context as ActionExecutingContext).HttpContext.Request;
                var body = request.Headers["Request-Body"].ToString();
                return CustomChecker.Check(body);
            }
            else if (CustomCheckerType != null)
            {
                var request = (context as ActionExecutingContext).HttpContext.Request;
                var body = request.Headers["Request-Body"].ToString();
                var checker = CustomCheckerType.Assembly.CreateInstance(CustomCheckerType.Name);
                return (CheckPermissionResult)CustomCheckerType.GetMethod("Check").Invoke(checker, new object[] { body });
            }
            return CheckPermissionResult.Skip;
        }
    }
}
