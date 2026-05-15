namespace highspeed.api.Filter
{
    public class AdapAction
    {
        private Action _Action;
        private Task _Task;
        private CancellationTokenSource _CancellationTokenSource;
        public Guid Guid { get; private set; }
        public IntPtr Handle => _Action.Method.MethodHandle.Value;
        public string Name { get; private set; }
        public string? State => _Task?.Status.ToString();
        public string? Message => _Task?.Exception?.Message;
        public object? Result { get; private set; }
        public int ProcessTotal { get; set; }
        public int ProcessCurrent { get; set; }
        public string ProcessTitle { get; set; }
        public string ProcessMessage { get; set; }

        private AdapAction()
        {
            Guid = Guid.NewGuid();
        }

        public static AdapAction New(string actionName, Action action)
        {
            var act = new AdapAction();
            act.Name = actionName;
            act._CancellationTokenSource = new CancellationTokenSource();
            var token = act._CancellationTokenSource.Token;
            act._Action = () =>
            {
                using (token.Register(Thread.CurrentThread.Abort))
                {
                    action();
                }
            };
            return act;
        }

        public static AdapAction New<T>(string actionName, Func<T> action)
        {
            var act = new AdapAction();
            act.Name = actionName;
            act._CancellationTokenSource = new CancellationTokenSource();
            var token = act._CancellationTokenSource.Token;
            act._Action = () =>
            {
                using (token.Register(Thread.CurrentThread.Abort))
                {
                    var r = action();
                    if (r is Task && r is IAsyncResult)
                    {
                        act._Task = r as Task;
                        act._Task.Wait();
                        act.Result = (r as dynamic).Result;
                    }
                    else
                        act.Result = r;
                }
            };
            return act;
        }

        public void Run()
        {
            _Task = Task.Run(_Action);
        }

        public void Cancel()
        {
            _CancellationTokenSource?.Cancel();
        }

        public void SetProcessInfo(int current, int? total = null, string? title = null, string? message = null)
        {
            ProcessCurrent = current;
            if (total != null) ProcessTotal = total.Value;
            if (title != null) ProcessTitle = title;
            if (message != null) ProcessMessage = message;
        }
    }
}
