using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace RideBoard.Widget.Services
{
    public class WindowConfigData
    {
        public double Top { get; set; } = -1;
        public double Left { get; set; } = -1;
        public int PageIndex { get; set; } = 0;
    }

    public static class WindowConfig
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "window_config.json");

        public static void Save(Window window)
        {
            try
            {
                var data = new WindowConfigData();
                
                // Save position
                if (window.WindowState == WindowState.Normal)
                {
                    data.Top = window.Top;
                    data.Left = window.Left;
                }
                
                // Save page index
                if (window.DataContext is RideBoard.Widget.ViewModels.WidgetViewModel vm)
                {
                    data.PageIndex = vm.CurrentPageIndex;
                }

                var json = JsonSerializer.Serialize(data);
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception) { /* Ignore errors during save */ }
        }

        public static void Load(Window window)
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    var data = JsonSerializer.Deserialize<WindowConfigData>(json);
                    if (data != null)
                    {
                        // Restore Position
                        if (data.Top != -1 && data.Left != -1)
                        {
                            // Basic bounds check to ensure window is visible on screen
                            if (data.Top >= SystemParameters.VirtualScreenTop &&
                                data.Top < SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight &&
                                data.Left >= SystemParameters.VirtualScreenLeft &&
                                data.Left < SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth)
                            {
                                window.Top = data.Top;
                                window.Left = data.Left;
                            }
                        }

                        // Restore Page Index
                        if (window.DataContext is RideBoard.Widget.ViewModels.WidgetViewModel vm)
                        {
                            vm.CurrentPageIndex = data.PageIndex;
                        }
                    }
                }
            }
            catch (Exception) { /* Ignore errors during load */ }
        }
    }
}
