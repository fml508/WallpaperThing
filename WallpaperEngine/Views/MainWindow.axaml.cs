using Avalonia;
using Avalonia.Controls;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace WallpaperEngine.Views
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter,
            string lpszClass, string? lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg,
            IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        // Kiegészítések az Alt+Tab eltüntetéséhez
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TOOLWINDOW = 0x00000080;
        const int WS_EX_NOACTIVATE = 0x08000000;

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        const uint WM_SPAWN_WORKER = 0x052C;
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOACTIVATE = 0x0010;

        public MainWindow()
        {
            InitializeComponent();
            this.IsHitTestVisible = false;

            var screen = Screens.Primary;
            if (screen != null)
            {
                this.Position = new PixelPoint(0, 0);
                this.Width = screen.WorkingArea.Width;
                this.Height = screen.WorkingArea.Height;
            }
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            var hwnd = this.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
            if (hwnd == IntPtr.Zero) return;

            // Alt+Tab eltüntetése és NoActivate beállítása
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            exStyle |= WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);

            IntPtr progman = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Progman", null);
            IntPtr shellDefView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);

            System.Diagnostics.Debug.WriteLine($"[WE] progman: {progman}, shellDefView: {shellDefView}");

            // Beágyazás a Progman-ba
            SetParent(hwnd, progman);

            // Az ablakot a SHELLDLL_DefView MÖGé toljuk a Progman gyermekei között
            if (shellDefView != IntPtr.Zero)
            {
                SetWindowPos(hwnd, shellDefView, 0, 0, 0, 0,
                             SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
                System.Diagnostics.Debug.WriteLine("[WE] Placed behind SHELLDLL_DefView");
            }
        }

        private static IntPtr FindWorkerW()
        {
            IntPtr progman = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Progman", null);

            SendMessageTimeout(progman, WM_SPAWN_WORKER, new IntPtr(0xD), new IntPtr(0x1), 0, 1000, out _);
            SendMessageTimeout(progman, WM_SPAWN_WORKER, new IntPtr(0xD), IntPtr.Zero, 0, 1000, out _);

            System.Threading.Thread.Sleep(500);

            IntPtr workerW = IntPtr.Zero;

            IntPtr desktop = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "WorkerW", null);
            while (desktop != IntPtr.Zero)
            {
                IntPtr shellDll = FindWindowEx(desktop, IntPtr.Zero, "SHELLDLL_DefView", null);
                System.Diagnostics.Debug.WriteLine($"[WE] WorkerW: {desktop}, SHELLDLL_DefView: {shellDll}");

                if (shellDll != IntPtr.Zero)
                {
                    workerW = FindWindowEx(IntPtr.Zero, desktop, "WorkerW", null);
                    System.Diagnostics.Debug.WriteLine($"[WE] Found icon WorkerW: {desktop}, target WorkerW: {workerW}");
                    break;
                }

                desktop = FindWindowEx(IntPtr.Zero, desktop, "WorkerW", null);
            }

            System.Diagnostics.Debug.WriteLine($"[WE] Final workerW: {workerW}");
            return workerW;
        }

        
    }
}