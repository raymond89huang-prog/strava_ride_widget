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
    }
}
