using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace RideBoard.Widget
{
    public partial class App : System.Windows.Application
    {
        private System.Windows.Forms.NotifyIcon? _notifyIcon;
        private Process? _serverProcess;

        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupTrayIcon();
            StartServer();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon?.Dispose();
            StopServer();
            base.OnExit(e);
        }

        private void SetupTrayIcon()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            try
            {
                _notifyIcon.Icon = CreateAppIcon();
            }
            catch 
            {
                _notifyIcon.Icon = System.Drawing.SystemIcons.Application;
            }
            
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "RideBoard";
            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();

            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Show Widget (显示挂件)", null, (s, args) => ShowMainWindow());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Exit (退出)", null, (s, args) => Shutdown());
            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void ShowMainWindow()
        {
            if (MainWindow != null)
            {
                MainWindow.Show();
                MainWindow.WindowState = WindowState.Normal;
                MainWindow.Activate();
            }
        }

        private void StartServer()
        {
            try
            {
                // Locate server.py reliably using BaseDirectory
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                // Path from bin/Debug/net10.0-windows/ to server/src/server.py
                string serverPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\..\server\src\server.py"));
                
                if (!File.Exists(serverPath))
                {
                    // Fallback: maybe running from project root during dev?
                    serverPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\server\src\server.py"));
                }
                
                // Fallback 2: Distribution/Publish mode (server folder is copied to output root)
                if (!File.Exists(serverPath))
                {
                     serverPath = Path.GetFullPath(Path.Combine(baseDir, @"server\src\server.py"));
                }

                if (File.Exists(serverPath))
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "python",
                        Arguments = $"\"{serverPath}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    _serverProcess = Process.Start(psi);
                }
                else
                {
                     // Debug.WriteLine($"Server not found at: {serverPath}");
                }
            }
            catch (Exception ex)
            {
                // Log or ignore, maybe python is not in PATH
                Debug.WriteLine($"Failed to start server: {ex.Message}");
            }
        }

        private void StopServer()
        {
            try
            {
                if (_serverProcess != null && !_serverProcess.HasExited)
                {
                    _serverProcess.Kill();
                    _serverProcess = null;
                }
            }
            catch { }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Log but don't show annoying popup for every error in production
            // System.Windows.MessageBox.Show("Unhandled Exception: " + e.Exception.Message, "RideBoard Error");
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                // System.Windows.MessageBox.Show("Critical Error: " + ex.Message, "RideBoard Fatal");
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        private System.Drawing.Icon CreateAppIcon()
        {
            using (var bitmap = new System.Drawing.Bitmap(64, 64))
            {
                using (var g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    // Draw background circle (Strava Orange)
                    using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(252, 76, 2)))
                    {
                        g.FillEllipse(brush, 0, 0, 64, 64);
                    }

                    // Draw "R" in White
                    using (var font = new System.Drawing.Font("Segoe UI", 40, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel))
                    {
                        var text = "R";
                        var size = g.MeasureString(text, font);
                        float x = (64 - size.Width) / 2;
                        float y = (64 - size.Height) / 2;
                        g.DrawString(text, font, System.Drawing.Brushes.White, x, y);
                    }
                }

                var hIcon = bitmap.GetHicon();
                var icon = System.Drawing.Icon.FromHandle(hIcon);
                var clonedIcon = (System.Drawing.Icon)icon.Clone();
                DestroyIcon(hIcon);
                return clonedIcon;
            }
        }
    }
}
