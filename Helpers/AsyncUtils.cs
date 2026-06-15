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

        /// <summary>
        /// Returns a debounced action. Successive calls within <paramref name="milliseconds"/> cancel the
        /// previous pending invocation. The wrapped <paramref name="action"/> runs on the synchronization
        /// context captured at the call site, so it is safe to touch UI elements from within.
        /// </summary>
        public static Action Debounce(Action action, int milliseconds = 300)
        {
            var ctx = SynchronizationContext.Current;
            var scheduler = ctx != null
                ? TaskScheduler.FromCurrentSynchronizationContext()
                : TaskScheduler.Default;
            CancellationTokenSource? cts = null;
            return () =>
            {
                cts?.Cancel();
                cts = new CancellationTokenSource();
                var token = cts.Token;
                _ = Task.Delay(milliseconds, token).ContinueWith(t =>
                {
                    if (t.IsCanceled || t.IsFaulted) return;
                    if (ctx != null) ctx.Post(_ => action(), null);
                    else action();
                }, scheduler);
            };
        }
    }
}

