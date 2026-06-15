using System;
using System.Threading.Tasks;

namespace winUItoolkit.Helpers
{
    public static class RetryPolicyHelper
    {
        /// <summary>
        /// Executes <paramref name="action"/> up to <paramref name="maxRetries"/> times with exponential backoff.
        /// On the final failure, returns <c>default</c>. If you need the exception, use the overload that throws.
        /// </summary>
        public static async Task<T?> ExecuteAsync<T>(Func<Task<T>> action, int maxRetries = 3, TimeSpan? baseDelay = null)
        {
            try
            {
                return await ExecuteOrThrowAsync(action, maxRetries, baseDelay);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Executes <paramref name="action"/> up to <paramref name="maxRetries"/> times, rethrowing the last exception on failure.
        /// </summary>
        public static async Task<T> ExecuteOrThrowAsync<T>(Func<Task<T>> action, int maxRetries = 3, TimeSpan? baseDelay = null)
        {
            TimeSpan delay = baseDelay ?? TimeSpan.FromMilliseconds(200);
            for (int attempt = 1; ; attempt++)
            {
                try
                {
                    return await action();
                }
                catch when (attempt < maxRetries)
                {
                    await Task.Delay(ComputeBackoff(attempt, delay));
                }
            }
        }

        /// <summary>
        /// Computes the exponential backoff delay for a given attempt number (1-based).
        /// </summary>
        public static TimeSpan ComputeBackoff(int attempt, TimeSpan baseDelay)
        {
            double multiplier = Math.Pow(2, Math.Max(0, attempt - 1));
            return TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * multiplier);
        }
    }
}
