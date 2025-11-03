using System;
using System.Threading.Tasks;

namespace winUItoolkit.Helpers
{
    public static class RetryPolicyHelper
    {
        public static async Task<T?> ExecuteAsync<T>(Func<Task<T>> action, int maxRetries = 3, TimeSpan? baseDelay = null)
        {
            baseDelay ??= TimeSpan.FromMilliseconds(200);
            for (int i = 1; i <= maxRetries; i++)
            {
                try
                {
                    return await action().ConfigureAwait(false);
                }
                catch when (i < maxRetries)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(baseDelay.Value.TotalMilliseconds * Math.Pow(2, i - 1))).ConfigureAwait(false);
                }
            }
            return default;
        }
    }
}
