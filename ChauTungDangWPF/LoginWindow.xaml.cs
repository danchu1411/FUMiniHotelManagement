using System.Windows;

namespace ChauTungDangWPF;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Login handler is not implemented yet.", "Login", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
