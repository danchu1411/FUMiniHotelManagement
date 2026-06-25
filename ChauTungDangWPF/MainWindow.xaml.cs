using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BusinessObjects;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace ChauTungDangWPF;

public partial class MainWindow : Window
{
    private readonly bool _isAdmin;
    private readonly string _displayName;
    private readonly FuminiHotelManagementContext _context;

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
        _context = new FuminiHotelManagementContext();
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
                LoadRoomData();
                break;

            case DashboardModule.Customer:
                txtModuleTitle.Text = "Customer Management";
                cboFilter.Items.Add("Customer Name");
                cboFilter.Items.Add("Email");
                cboFilter.Items.Add("Phone Number");
                LoadCustomerData();
                break;

            case DashboardModule.Order:
                txtModuleTitle.Text = "Order Management";
                cboFilter.Items.Add("Booking ID");
                cboFilter.Items.Add("Customer Name");
                cboFilter.Items.Add("Status");
                LoadOrderData();
                break;

            case DashboardModule.Report:
                txtModuleTitle.Text = "Report Management";
                cboFilter.Items.Add("Start Date");
                cboFilter.Items.Add("End Date");
                break;
        }

        cboFilter.SelectedIndex = 0;
    }

    private void LoadRoomData()
    {
        try
        {
            dgData.Columns.Clear();

            dgData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("RoomId"), Width = 60 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Room Number", Binding = new Binding("RoomNumber"), Width = 120 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Description", Binding = new Binding("RoomDetailDescription"), Width = 200 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Price/Day", Binding = new Binding("RoomPricePerDay"), Width = 100 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Status", Binding = new Binding("RoomStatus"), Width = 100 });

            var data = _context.RoomInformations.ToList();
            dgData.ItemsSource = data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading rooms: {ex.Message}", "Error");
        }
    }

    private void LoadCustomerData()
    {
        try
        {
            dgData.Columns.Clear();

            dgData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("CustomerId"), Width = 60 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Full Name", Binding = new Binding("CustomerFullName"), Width = 150 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new Binding("EmailAddress"), Width = 200 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Phone", Binding = new Binding("Telephone"), Width = 120 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Status", Binding = new Binding("CustomerStatus"), Width = 100 });

            var data = _context.Customers.ToList();
            dgData.ItemsSource = data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading customers: {ex.Message}", "Error");
        }
    }

    private void LoadOrderData()
    {
        try
        {
            dgData.Columns.Clear();

            dgData.Columns.Add(new DataGridTextColumn { Header = "Booking ID", Binding = new Binding("BookingReservationId"), Width = 100 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Customer", Binding = new Binding("Customer.CustomerFullName"), Width = 150 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Booking Date", Binding = new Binding("BookingDate"), Width = 120 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Total Price", Binding = new Binding("TotalPrice"), Width = 120 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Status", Binding = new Binding("BookingStatus"), Width = 100 });

            var data = _context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                .ToList();
            dgData.ItemsSource = data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading orders: {ex.Message}", "Error");
        }
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
