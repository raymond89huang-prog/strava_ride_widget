using Microsoft.Win32;
using System;
using System.IO;

namespace RideBoard.Widget.Services
{
    public static class StartupManager
    {
        private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "RideBoardWidget";

        public static bool IsEnabled
        {
            get
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(RunKey, false);
                    return key?.GetValue(AppName) != null;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static void Set(bool enable)
        {
            if (enable) Enable();
            else Disable();
        }

        private static void Enable()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKey, true);
                // Ensure we quote the path in case of spaces
                var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
                if (!string.IsNullOrEmpty(exePath))
                {
                    key?.SetValue(AppName, $"\"{exePath}\"");
                }
            }
            catch { }
        }

        private static void Disable()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKey, true);
                key?.DeleteValue(AppName, false);
            }
            catch { }
        }
    }
}
