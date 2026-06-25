using System;
using System.Text.RegularExpressions;
using System.Windows;
using BusinessObjects;
using Services;

namespace ChauTungDangWPF;

public partial class CreateCustomerWindow : Window
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex PhoneRegex = new(@"^\d{10,12}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly ICustomerService _customerService;

    public CreateCustomerWindow()
    {
        InitializeComponent();
        _customerService = new CustomerService();
        dpBirthday.DisplayDateEnd = DateTime.Today.AddYears(-16);
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput())
            return;

        try
        {
            var email = txtEmail.Text.Trim();
            var phone = txtPhone.Text.Trim();
            DateOnly? birthday = dpBirthday.SelectedDate.HasValue
                ? DateOnly.FromDateTime(dpBirthday.SelectedDate.Value)
                : null;

            if (_customerService.GetCustomerByEmail(email) != null)
            {
                MessageBox.Show("This email is already registered.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newCustomer = new Customer
            {
                CustomerFullName = txtFullName.Text.Trim(),
                EmailAddress = email,
                Telephone = phone,
                CustomerBirthday = birthday,
                Password = pwdPassword.Password,
                CustomerStatus = 1
            };

            _customerService.AddCustomer(newCustomer);

            MessageBox.Show("Customer created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidateInput()
    {
        var fullName = txtFullName.Text.Trim();
        var email = txtEmail.Text.Trim();
        var phone = txtPhone.Text.Trim();
        var password = pwdPassword.Password;

        if (string.IsNullOrWhiteSpace(fullName))
        {
            MessageBox.Show("Please enter a full name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            MessageBox.Show("Please enter an email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (!EmailRegex.IsMatch(email))
        {
            MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            MessageBox.Show("Please enter a phone number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (!PhoneRegex.IsMatch(phone))
        {
            MessageBox.Show("Phone number must contain only digits and be 10 to 12 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Please enter a password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (!dpBirthday.SelectedDate.HasValue)
        {
            MessageBox.Show("Please select a birthday.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        var birthday = dpBirthday.SelectedDate.Value.Date;
        var today = DateTime.Today;

        if (birthday > today)
        {
            MessageBox.Show("Birthday cannot be in the future.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        var age = today.Year - birthday.Year;
        if (birthday.Date > today.AddYears(-age))
            age--;

        if (age < 16)
        {
            MessageBox.Show("Customer must be at least 16 years old.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
