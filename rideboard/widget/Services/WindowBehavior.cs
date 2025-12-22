using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace RideBoard.Widget.Services
{
    public static class WindowBehavior
    {
        public static void SnapToEdges(Window w, int threshold)
        {
            var screen = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(w).Handle);
            var wa = screen.WorkingArea;
            var left = w.Left;
            var top = w.Top;
            var right = w.Left + w.Width;
            var bottom = w.Top + w.Height;
            if (Math.Abs(left - wa.Left) <= threshold) w.Left = wa.Left;
            if (Math.Abs(top - wa.Top) <= threshold) w.Top = wa.Top;
            if (Math.Abs(wa.Right - right) <= threshold) w.Left = wa.Right - w.Width;
            if (Math.Abs(wa.Bottom - bottom) <= threshold) w.Top = wa.Bottom - w.Height;
        }

        public static void ApplyClickThrough(Window w, bool enable)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(w).Handle;
            var exStyle = GetWindowLong(hwnd, GwlExstyle);
            if (enable)
            {
                SetWindowLong(hwnd, GwlExstyle, exStyle | WsExTransparent | WsExLayered);
            }
            else
            {
                SetWindowLong(hwnd, GwlExstyle, exStyle & ~WsExTransparent);
            }
        }

        private const int GwlExstyle = -20;
        private const int WsExTransparent = 0x00000020;
        private const int WsExLayered = 0x00080000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}

