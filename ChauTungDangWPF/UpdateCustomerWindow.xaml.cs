using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using BusinessObjects;
using DataAccessLayer;

namespace ChauTungDangWPF;

public partial class UpdateCustomerWindow : Window
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex PhoneRegex = new(@"^\d{10,12}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly FuminiHotelManagementContext _context;
    private readonly Customer _customer;

    public UpdateCustomerWindow(Customer customer)
    {
        InitializeComponent();
        _context = new FuminiHotelManagementContext();
        _customer = customer;
        dpBirthday.DisplayDateEnd = DateTime.Today.AddYears(-16);
        LoadCustomerData();
    }

    private void LoadCustomerData()
    {
        txtFullName.Text = _customer.CustomerFullName;
        txtEmail.Text = _customer.EmailAddress;
        txtPhone.Text = _customer.Telephone;
        pwdPassword.Password = _customer.Password ?? string.Empty;

        if (_customer.CustomerBirthday.HasValue)
        {
            dpBirthday.SelectedDate = _customer.CustomerBirthday.Value.ToDateTime(TimeOnly.MinValue);
        }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput())
            return;

        try
        {
            var email = txtEmail.Text.Trim();
            var phone = txtPhone.Text.Trim();
            var birthday = DateOnly.FromDateTime(dpBirthday.SelectedDate!.Value);

            var normalizedEmail = email.ToLower();
            var duplicateCustomer = _context.Customers.FirstOrDefault(c =>
                c.CustomerId != _customer.CustomerId &&
                c.EmailAddress != null &&
                c.EmailAddress.ToLower() == normalizedEmail);

            if (duplicateCustomer != null)
            {
                MessageBox.Show("This email is already registered.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existingCustomer = _context.Customers.FirstOrDefault(c => c.CustomerId == _customer.CustomerId);
            if (existingCustomer == null)
            {
                MessageBox.Show("Customer not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            existingCustomer.CustomerFullName = txtFullName.Text.Trim();
            existingCustomer.EmailAddress = email;
            existingCustomer.Telephone = phone;
            existingCustomer.CustomerBirthday = birthday;
            existingCustomer.Password = pwdPassword.Password;

            _context.SaveChanges();

            MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
