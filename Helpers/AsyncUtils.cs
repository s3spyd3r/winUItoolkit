using System;
using System.Threading;
using System.Threading.Tasks;

namespace winUItoolkit.Helpers
{
    public static class AsyncUtils
    {
        public static void FireAndForget(Func<Task> taskFunc, Action<Exception>? onError = null)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await taskFunc().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex);
                }
            });
        }

        public static Action Debounce(Action action, int milliseconds = 300)
        {
            CancellationTokenSource? cts = null;
            return () =>
            {
                cts?.Cancel();
                cts = new CancellationTokenSource();
                var token = cts.Token;
                Task.Delay(milliseconds, token).ContinueWith(t =>
                {
                    if (!t.IsCanceled) action();
                }, TaskScheduler.Default);
            };
        }
    }
}
