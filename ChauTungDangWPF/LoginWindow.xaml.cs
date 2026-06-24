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
            if (_customerService.IsAdmin(email, password))
            {
                MessageBox.Show("Đăng nhập Admin thành công. Main Window đang implement.", "Login thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var customer = _customerService.Login(email, password);
            if (customer != null)
            {
                string customerName = string.IsNullOrWhiteSpace(customer.CustomerFullName) ? customer.EmailAddress : customer.CustomerFullName;
                MessageBox.Show($"Đăng nhập thành công. Xin chào {customerName}. Main Window đang implement.", "Login thành công", MessageBoxButton.OK, MessageBoxImage.Information);
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
