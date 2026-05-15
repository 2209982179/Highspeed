using Microsoft.AspNetCore.Mvc;

namespace highspeed.api.Filter
{
    public class ActionResponse : ObjectResult
    {
        public Guid? Guid { get; private set; }

        public static ActionResponse New(string actionName, Action action)
        {
            return new ActionResponse(ActionCenter.Push(actionName, action));
        }

        public static ActionResponse New<T>(string actionName, Func<T> action)
        {
            return new ActionResponse(ActionCenter.Push(actionName, action));
        }

        private ActionResponse(Guid guid) : base(guid.ToString())
        {
            Guid = guid;
        }
    }

    public static class ControllerBaseTaskExtensions
    {
        public static ActionResponse Action(this ControllerBase controller, string actionName, Action action)
        {
            return ActionResponse.New(actionName, action);
        }
        public static ActionResponse Action<T>(this ControllerBase controller, string actionName, Func<T> action)
        {
            return ActionResponse.New(actionName, action);
        }
    }
}
