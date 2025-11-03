using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace winUItoolkit.Helpers
{
    public static class PerformanceHelper
    {
        public static T Measure<T>(Func<T> func, out TimeSpan elapsed)
        {
            var sw = Stopwatch.StartNew();
            var result = func();
            sw.Stop();
            elapsed = sw.Elapsed;
            return result;
        }

        public static async Task<T> MeasureAsync<T>(Func<Task<T>> func, Action<TimeSpan>? onMeasured = null)
        {
            var sw = Stopwatch.StartNew();
            var result = await func().ConfigureAwait(false);
            sw.Stop();
            onMeasured?.Invoke(sw.Elapsed);
            return result;
        }
    }
}