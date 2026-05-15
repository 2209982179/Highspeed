namespace highspeed.api.Filter
{
    public static class ActionCenter
    {
        private static object locker = new object();
        private static Dictionary<Guid, AdapAction> ActionPool = new Dictionary<Guid, AdapAction>();

        public static Guid Push(string actionName, Action action)
        {
            lock (locker)
            {
                var act = AdapAction.New(actionName, action);
                ActionPool.Add(act.Guid, act);
                act.Run();
                return act.Guid;
            }
        }
        public static Guid Push<T>(string actionName, Func<T> action)
        {
            lock (locker)
            {
                var act = AdapAction.New(actionName, action);
                ActionPool.Add(act.Guid, act);
                act.Run();
                return act.Guid;
            }
        }

        public static AdapAction? Find(Guid actionGuid)
        {
            if (ActionPool.ContainsKey(actionGuid))
                lock (locker)
                {
                    if (ActionPool.ContainsKey(actionGuid)) return ActionPool[actionGuid];
                    return null;
                }
            return null;
        }

        public static void Remove(Guid actionGuid)
        {
            if (ActionPool.ContainsKey(actionGuid))
                lock (locker)
                {
                    if (ActionPool.ContainsKey(actionGuid)) ActionPool.Remove(actionGuid);
                }
        }
    }
}
