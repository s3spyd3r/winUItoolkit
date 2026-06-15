using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Helpers;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class MiscPage : UserControl
    {
        private readonly Action _debounced;

        public MiscPage()
        {
            InitializeComponent();
            _debounced = AsyncUtils.Debounce(() =>
            {
                DebounceResult.Text = $"Debounced at {DateTime.Now:HH:mm:ss.fff}";
            }, 400);
        }

        private void Measure_Click(object sender, RoutedEventArgs e)
        {
            _ = PerformanceHelper.Measure(() =>
            {
                long s = 0;
                for (int i = 0; i < 5_000_000; i++) s += i;
                return s;
            }, out TimeSpan elapsed);
            MeasureResult.Text = $"Elapsed: {elapsed.TotalMilliseconds:0.0} ms";
        }

        private void LogInfo_Click(object sender, RoutedEventArgs e)
            => LoggingHelper.Info("This is an info entry from the example app.");

        private void LogWarn_Click(object sender, RoutedEventArgs e)
            => LoggingHelper.Warn("This is a warning entry from the example app.");

        private void LogError_Click(object sender, RoutedEventArgs e)
            => LoggingHelper.Error("This is an error entry from the example app.");

        private void Debounce_TextChanged(object sender, TextChangedEventArgs e)
            => _debounced();
    }
}
