using System.Windows;
using Services;

namespace ChauTungDangWPF;

public partial class LoginWindow : Window
{
    private readonly ICustomerService _customerService;

    public LoginWindow()
    {
        InitializeComponent();
        _customerService = new CustomerService();
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        string email = txtUser.Text.Trim();
        string password = txtPass.Password;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Vui lòng nhập đầy đủ Email và Password.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            bool isAdmin = _customerService.IsAdmin(email, password);

            if (isAdmin)
            {
                var MainWindow = new MainWindow(isAdmin: true, displayName: "Admin");
                MainWindow.Show();
                Close();
                return;
            }

            var customer = _customerService.Login(email, password);
            if (customer != null)
            {
                string displayName = string.IsNullOrWhiteSpace(customer.CustomerFullName) ? customer.EmailAddress : customer.CustomerFullName;
                var MainWindow = new MainWindow(isAdmin: false, displayName: displayName);
                MainWindow.Show();
                Close();
                return;
            }

            MessageBox.Show("Email hoặc Password không đúng.", "Đăng nhập thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể đăng nhập lúc này. Chi tiết: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
