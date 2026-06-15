using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Helpers;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class RetryPage : UserControl
    {
        public RetryPage()
        {
            InitializeComponent();
        }

        private async void Run_Click(object sender, RoutedEventArgs e)
        {
            int attempt = 0;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var result = await RetryPolicyHelper.ExecuteOrThrowAsync<string>(async () =>
            {
                attempt++;
                Result.Text = $"Attempt {attempt}...";
                await Task.Delay(200);
                if (attempt < 3) throw new InvalidOperationException("Flaky failure.");
                return $"succeeded on attempt {attempt}";
            }, maxRetries: 5, baseDelay: TimeSpan.FromMilliseconds(150));
            sw.Stop();
            Result.Text = $"{result} (total: {sw.ElapsedMilliseconds} ms)";
        }
    }
}
