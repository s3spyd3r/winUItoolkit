using System;
using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Helpers;
using WinRT.Interop;

namespace WinUIToolkit.Examples
{
    public sealed partial class MainWindow : Window
    {
        private const int MinWidth = 800;
        private const int MinHeight = 480;

        public MainWindow()
        {
            InitializeComponent();
            Title = "winUItoolkit Examples";
            EnforceMinimumSize();
        }

        private void EnforceMinimumSize()
        {
            var hwnd = WindowNative.GetWindowHandle(this);
            SubclassForMinSize(hwnd);
        }

        private static void SubclassForMinSize(IntPtr hwnd)
        {
            var original = SetWindowLongPtr(hwnd, GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate<WndProc>(WndProcHook));
            _originalProcs[hwnd] = original;
        }

        private static readonly System.Collections.Generic.Dictionary<IntPtr, IntPtr> _originalProcs = new();

        private delegate IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        private static IntPtr WndProcHook(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            const int WM_GETMINMAXINFO = 0x0024;
            if (msg == WM_GETMINMAXINFO)
            {
                var info = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                info.ptMinTrackSize.x = MinWidth;
                info.ptMinTrackSize.y = MinHeight;
                Marshal.StructureToPtr(info, lParam, true);
            }

            return CallWindowProc(_originalProcs[hwnd], hwnd, msg, wParam, lParam);
        }

        #region Win32

        private const int GWLP_WNDPROC = -4;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
        private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
            => IntPtr.Size == 8 ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : SetWindowLong32(hWnd, nIndex, dwNewLong);

        [DllImport("user32.dll")]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        #endregion

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not NavigationViewItem item) return;
            if (item.Tag is not string tag) return;

            if (tag == "home")
            {
                ContentFrame.Content = new HomePage();
                return;
            }

            var pageType = System.Type.GetType(tag);
            if (pageType != null)
            {
                ContentFrame.Content = Activator.CreateInstance(pageType);
            }
        }
    }
}
