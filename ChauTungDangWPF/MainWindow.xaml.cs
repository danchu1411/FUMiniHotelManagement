using System;
using System.Collections.Generic;
using System.Linq;
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
    private bool _isInitializing = false;

    private IEnumerable<RoomInformation> _allRooms = new List<RoomInformation>();
    private IEnumerable<Customer> _allCustomers = new List<Customer>();
    private IEnumerable<BookingReservation> _allOrders = new List<BookingReservation>();

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
        _isInitializing = true;
        _currentModule = module;

        cboFilter.Items.Clear();
        txtSearch.Text = string.Empty;

        switch (module)
        {
            case DashboardModule.Room:
                txtModuleTitle.Text = "Room Management";
                cboFilter.Items.Add("All");
                cboFilter.Items.Add("Room Number");
                cboFilter.Items.Add("Room Type");
                cboFilter.Items.Add("Status");
                LoadRoomData();
                break;

            case DashboardModule.Customer:
                txtModuleTitle.Text = "Customer Management";
                cboFilter.Items.Add("All");
                cboFilter.Items.Add("Customer Name");
                cboFilter.Items.Add("Email");
                cboFilter.Items.Add("Phone Number");
                LoadCustomerData();
                break;

            case DashboardModule.Order:
                txtModuleTitle.Text = "Order Management";
                cboFilter.Items.Add("All");
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
        _isInitializing = false;
    }

    private void LoadRoomData()
    {
        try
        {
            dgData.Columns.Clear();

            dgData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("RoomId"), Width = 60 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Room Number", Binding = new Binding("RoomNumber"), Width = 120 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Room Type", Binding = new Binding("RoomType.RoomTypeName"), Width = 120 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Description", Binding = new Binding("RoomDetailDescription"), Width = 200 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Price/Day", Binding = new Binding("RoomPricePerDay"), Width = 100 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Status", Binding = new Binding("RoomStatus"), Width = 100 });

            _allRooms = _context.RoomInformations.Include(r => r.RoomType).ToList();
            dgData.ItemsSource = _allRooms;
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

            _allCustomers = _context.Customers.ToList();
            dgData.ItemsSource = _allCustomers;
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

            _allOrders = _context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                .ToList();
            dgData.ItemsSource = _allOrders;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading orders: {ex.Message}", "Error");
        }
    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isInitializing)
            return;

        if (string.IsNullOrEmpty(txtSearch.Text) || txtSearch.Text == "Search keyword or date...")
            return;

        ApplyFilter();
    }

    private void cboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isInitializing)
            return;

        if (cboFilter.SelectedItem == null)
            return;

        ApplyFilter();
    }

    private void btnFind_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        try
        {
            string searchText = (txtSearch.Text ?? string.Empty).ToLower().Trim();

            if (searchText == "search keyword or date..." || string.IsNullOrEmpty(searchText))
            {
                switch (_currentModule)
                {
                    case DashboardModule.Room:
                        if (_allRooms != null)
                            dgData.ItemsSource = _allRooms.ToList();
                        break;
                    case DashboardModule.Customer:
                        if (_allCustomers != null)
                            dgData.ItemsSource = _allCustomers.ToList();
                        break;
                    case DashboardModule.Order:
                        if (_allOrders != null)
                            dgData.ItemsSource = _allOrders.ToList();
                        break;
                }
                return;
            }

            string filterType = cboFilter.SelectedItem?.ToString() ?? "All";

            switch (_currentModule)
            {
                case DashboardModule.Room:
                    if (_allRooms != null)
                        FilterRooms(searchText, filterType);
                    break;

                case DashboardModule.Customer:
                    if (_allCustomers != null)
                        FilterCustomers(searchText, filterType);
                    break;

                case DashboardModule.Order:
                    if (_allOrders != null)
                        FilterOrders(searchText, filterType);
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error applying filter: {ex.Message}", "Error");
        }
    }

    private void FilterRooms(string searchText, string filterType)
    {
        if (_allRooms == null || !_allRooms.Any())
        {
            dgData.ItemsSource = null;
            return;
        }

        try
        {
            var filtered = _allRooms.AsEnumerable();

            if (!string.IsNullOrEmpty(searchText))
            {
                if (filterType == "Room Number")
                {
                    filtered = filtered.Where(r => r.RoomNumber != null && r.RoomNumber.ToLower().Contains(searchText));
                }
                else if (filterType == "Room Type")
                {
                    filtered = filtered.Where(r => r.RoomType != null && r.RoomType.RoomTypeName != null && r.RoomType.RoomTypeName.ToLower().Contains(searchText));
                }
                else if (filterType == "Status")
                {
                    filtered = filtered.Where(r => r.RoomStatus != null && r.RoomStatus.ToString().Contains(searchText));
                }
                else // "All" or any other case
                {
                    filtered = filtered.Where(r =>
                        (r.RoomNumber != null && r.RoomNumber.ToLower().Contains(searchText)) ||
                        (r.RoomType != null && r.RoomType.RoomTypeName != null && r.RoomType.RoomTypeName.ToLower().Contains(searchText)) ||
                        (r.RoomDetailDescription != null && r.RoomDetailDescription.ToLower().Contains(searchText)) ||
                        (r.RoomStatus != null && r.RoomStatus.ToString().Contains(searchText)));
                }
            }

            dgData.ItemsSource = filtered.ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error in FilterRooms: {ex.Message}", "Error");
        }
    }

    private void FilterCustomers(string searchText, string filterType)
    {
        if (_allCustomers == null || !_allCustomers.Any())
        {
            dgData.ItemsSource = null;
            return;
        }

        try
        {
            var filtered = _allCustomers.AsEnumerable();

            if (!string.IsNullOrEmpty(searchText))
            {
                if (filterType == "Customer Name")
                {
                    filtered = filtered.Where(c => c.CustomerFullName != null && c.CustomerFullName.ToLower().Contains(searchText));
                }
                else if (filterType == "Email")
                {
                    filtered = filtered.Where(c => c.EmailAddress != null && c.EmailAddress.ToLower().Contains(searchText));
                }
                else if (filterType == "Phone Number")
                {
                    filtered = filtered.Where(c => c.Telephone != null && c.Telephone.ToLower().Contains(searchText));
                }
                else // "All" or any other case
                {
                    filtered = filtered.Where(c =>
                        (c.CustomerFullName != null && c.CustomerFullName.ToLower().Contains(searchText)) ||
                        (c.EmailAddress != null && c.EmailAddress.ToLower().Contains(searchText)) ||
                        (c.Telephone != null && c.Telephone.ToLower().Contains(searchText)));
                }
            }

            dgData.ItemsSource = filtered.ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error in FilterCustomers: {ex.Message}", "Error");
        }
    }

    private void FilterOrders(string searchText, string filterType)
    {
        if (_allOrders == null || !_allOrders.Any())
        {
            dgData.ItemsSource = null;
            return;
        }

        try
        {
            var filtered = _allOrders.AsEnumerable();

            if (!string.IsNullOrEmpty(searchText))
            {
                if (filterType == "Booking ID")
                {
                    filtered = filtered.Where(o => o.BookingReservationId.ToString().Contains(searchText));
                }
                else if (filterType == "Customer Name")
                {
                    filtered = filtered.Where(o => o.Customer != null && o.Customer.CustomerFullName != null && o.Customer.CustomerFullName.ToLower().Contains(searchText));
                }
                else if (filterType == "Status")
                {
                    filtered = filtered.Where(o => o.BookingStatus != null && o.BookingStatus.ToString().Contains(searchText));
                }
                else // "All" or any other case
                {
                    filtered = filtered.Where(o =>
                        o.BookingReservationId.ToString().Contains(searchText) ||
                        (o.Customer != null && o.Customer.CustomerFullName != null && o.Customer.CustomerFullName.ToLower().Contains(searchText)) ||
                        (o.BookingStatus != null && o.BookingStatus.ToString().Contains(searchText)));
                }
            }

            dgData.ItemsSource = filtered.ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error in FilterOrders: {ex.Message}", "Error");
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
