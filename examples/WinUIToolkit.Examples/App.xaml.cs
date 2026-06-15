using Microsoft.UI.Xaml;
using winUItoolkit.Helpers;

namespace WinUIToolkit.Examples
{
    public partial class App : Application
    {
        public static Window? MainWindow { get; private set; }

        public App()
        {
            InitializeComponent();
            LoggingHelper.Init();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow();
            MainWindow.Activate();
        }
    }
}
