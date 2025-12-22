using System.Windows;
using RideBoard.Widget.ViewModels;

namespace RideBoard.Widget
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(WidgetViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                e.Handled = true;
            }
            catch { }
        }
    }
}
