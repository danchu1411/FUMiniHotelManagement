using System.Windows;

namespace ChauTungDangWPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void btnLogout_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Logout flow is not implemented yet.", "Logout", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
