using System;
using System.Windows;
using System.Windows.Controls;

namespace ChauTungDangWPF;

public partial class MainWindow : Window
{
    private readonly bool _isAdmin;
    private readonly string _displayName;

    private enum DashboardModule
    {
        Room,
        Customer,
        Order,
        Report
    }

    private DashboardModule _currentModule;

    public MainWindow(bool isAdmin, string displayName)
    {
        InitializeComponent();
        _isAdmin = isAdmin;
        _displayName = displayName;
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        txtCurrentUser.Text = _displayName;

        if (!_isAdmin)
        {
            btnReport.Visibility = Visibility.Collapsed;
        }

        LoadModule(DashboardModule.Room);
    }

    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string tag)
            return;

        if (!Enum.TryParse(tag, out DashboardModule module))
            return;

        if (!_isAdmin && module == DashboardModule.Report)
            return;

        LoadModule(module);
    }

    private void LoadModule(DashboardModule module)
    {
        _currentModule = module;

        cboFilter.Items.Clear();
        txtSearch.Text = string.Empty;

        switch (module)
        {
            case DashboardModule.Room:
                txtModuleTitle.Text = "Room Management";
                cboFilter.Items.Add("Room Number");
                cboFilter.Items.Add("Room Type");
                cboFilter.Items.Add("Status");
                break;

            case DashboardModule.Customer:
                txtModuleTitle.Text = "Customer Management";
                cboFilter.Items.Add("Customer Name");
                cboFilter.Items.Add("Email");
                cboFilter.Items.Add("Phone Number");
                break;

            case DashboardModule.Order:
                txtModuleTitle.Text = "Order Management";
                cboFilter.Items.Add("Booking ID");
                cboFilter.Items.Add("Customer Name");
                cboFilter.Items.Add("Status");
                break;

            case DashboardModule.Report:
                txtModuleTitle.Text = "Report Management";
                cboFilter.Items.Add("Start Date");
                cboFilter.Items.Add("End Date");
                break;
        }

        cboFilter.SelectedIndex = 0;
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void btnLogout_Click(object sender, RoutedEventArgs e)
    {
        var login = new LoginWindow();
        login.Show();
        Close();
    }
}
