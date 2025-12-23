using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using RideBoard.Widget.ViewModels;
using RideBoard.Widget.Services;

namespace RideBoard.Widget
{
    public partial class MainWindow : Window
    {
        private readonly WidgetViewModel _vm = new WidgetViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm;
            System.Windows.Application.Current.Exit += (s, e) => WindowConfig.Save(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowConfig.Load(this);
            // Topmost = true; // Disabled by user request
            WindowBehavior.ApplyClickThrough(this, false);
            _vm.Start();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source)
            {
                while (source != null && !(source is System.Windows.Controls.Button))
                {
                    source = System.Windows.Media.VisualTreeHelper.GetParent(source);
                }
                if (source is System.Windows.Controls.Button) return;
            }

            try
            {
                DragMove();
                WindowConfig.Save(this); // Save immediately after drag
            }
            catch { }
            WindowBehavior.SnapToEdges(this, 20);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            // Instead of Shutdown, we just Hide (since we have Tray Icon now)
            // But user might want to really close.
            // Let's hide it and let Tray Icon handle Exit.
            // Or, keep behavior as Shutdown if Tray Icon isn't implemented fully yet.
            // Plan said: "You can minimize/close the main window, and the app will keep running."
            Hide();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow(_vm);
            settings.Owner = this;
            settings.ShowDialog();
        }
    }
}
