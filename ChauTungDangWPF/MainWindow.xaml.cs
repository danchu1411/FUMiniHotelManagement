using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
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

    // Pagination
    private const int ITEMS_PER_PAGE = 10;
    private int _currentPage = 1;
    private int _totalPages = 1;
    private List<object> _currentPageData = new List<object>();

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
            ResetPagination();
            DisplayPage(_allRooms.Cast<object>().ToList());
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
            ResetPagination();
            DisplayPage(_allCustomers.Cast<object>().ToList());
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
            ResetPagination();
            DisplayPage(_allOrders.Cast<object>().ToList());
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
        {
            ResetPagination();
            switch (_currentModule)
            {
                case DashboardModule.Room:
                    if (_allRooms != null && _allRooms.Any())
                        DisplayPage(_allRooms.Cast<object>().ToList());
                    break;
                case DashboardModule.Customer:
                    if (_allCustomers != null && _allCustomers.Any())
                        DisplayPage(_allCustomers.Cast<object>().ToList());
                    break;
                case DashboardModule.Order:
                    if (_allOrders != null && _allOrders.Any())
                        DisplayPage(_allOrders.Cast<object>().ToList());
                    break;
            }
            return;
        }

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
        if (_isInitializing)
            return;

        // Skip if no search text
        if (string.IsNullOrEmpty(txtSearch.Text) || txtSearch.Text == "Search keyword or date...")
        {
            // This should not happen now, but just in case
            ResetPagination();
            switch (_currentModule)
            {
                case DashboardModule.Room:
                    if (_allRooms != null && _allRooms.Any())
                        DisplayPage(_allRooms.Cast<object>().ToList());
                    break;
                case DashboardModule.Customer:
                    if (_allCustomers != null && _allCustomers.Any())
                        DisplayPage(_allCustomers.Cast<object>().ToList());
                    break;
                case DashboardModule.Order:
                    if (_allOrders != null && _allOrders.Any())
                        DisplayPage(_allOrders.Cast<object>().ToList());
                    break;
            }
            return;
        }

        try
        {
            string searchText = txtSearch.Text.ToLower().Trim();
            string filterType = cboFilter.SelectedItem?.ToString() ?? "All";

            switch (_currentModule)
            {
                case DashboardModule.Room:
                    if (_allRooms != null && _allRooms.Any())
                        FilterRooms(searchText, filterType);
                    break;

                case DashboardModule.Customer:
                    if (_allCustomers != null && _allCustomers.Any())
                        FilterCustomers(searchText, filterType);
                    break;

                case DashboardModule.Order:
                    if (_allOrders != null && _allOrders.Any())
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
                else 
                {
                    filtered = filtered.Where(r =>
                        (r.RoomNumber != null && r.RoomNumber.ToLower().Contains(searchText)) ||
                        (r.RoomType != null && r.RoomType.RoomTypeName != null && r.RoomType.RoomTypeName.ToLower().Contains(searchText)) ||
                        (r.RoomDetailDescription != null && r.RoomDetailDescription.ToLower().Contains(searchText)) ||
                        (r.RoomStatus != null && r.RoomStatus.ToString().Contains(searchText)));
                }
            }

            ResetPagination();
            DisplayPage(filtered.Cast<object>().ToList());
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
                else
                {
                    filtered = filtered.Where(c =>
                        (c.CustomerFullName != null && c.CustomerFullName.ToLower().Contains(searchText)) ||
                        (c.EmailAddress != null && c.EmailAddress.ToLower().Contains(searchText)) ||
                        (c.Telephone != null && c.Telephone.ToLower().Contains(searchText)));
                }
            }

            ResetPagination();
            DisplayPage(filtered.Cast<object>().ToList());
        }
        catch (Exception ex)
        {

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
                else
                {
                    filtered = filtered.Where(o =>
                        o.BookingReservationId.ToString().Contains(searchText) ||
                        (o.Customer != null && o.Customer.CustomerFullName != null && o.Customer.CustomerFullName.ToLower().Contains(searchText)) ||
                        (o.BookingStatus != null && o.BookingStatus.ToString().Contains(searchText)));
                }
            }

            ResetPagination();
            DisplayPage(filtered.Cast<object>().ToList());
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

    private void btnCreate_Click(object sender, RoutedEventArgs e)
    {
        if (_currentModule == DashboardModule.Room)
        {
            var createWindow = new CreateRoomWindow();
            bool? result = createWindow.ShowDialog();

            if (result == true)
            {
                LoadRoomData();
            }
        }
        else if (_currentModule == DashboardModule.Customer)
        {
            var createWindow = new CreateCustomerWindow
            {
                Owner = this
            };
            bool? result = createWindow.ShowDialog();

            if (result == true)
            {
                LoadCustomerData();
            }
        }
        else
        {
            MessageBox.Show("Create functionality is only available for Rooms and Customers.", "Info");
        }
    }

    private void btnLogout_Click(object sender, RoutedEventArgs e)
    {
        var login = new LoginWindow();
        login.Show();
        Close();
    }

    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (_currentModule != DashboardModule.Room)
        {
            MessageBox.Show("Update functionality is only available for Rooms.", "Info");
            return;
        }

        if (dgData.SelectedItem is not RoomInformation room)
        {
            MessageBox.Show("Please select a room to update.", "Warning");
            return;
        }

        var updateWindow = new UpdateRoomWindow(room)
        {
            Owner = this
        };

        bool? result = updateWindow.ShowDialog();
        if (result == true)
        {
            LoadRoomData();
        }
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (dgData.SelectedItem == null)
        {
            MessageBox.Show("Please select an item to delete.", "Warning");
            return;
        }

        var result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            switch (_currentModule)
            {
                case DashboardModule.Room:
                    DeleteRoom();
                    break;
                case DashboardModule.Customer:
                    DeleteCustomer();
                    break;
                case DashboardModule.Order:
                    DeleteOrder();
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting item: {ex.Message}", "Error");
        }
    }

    private void DeleteRoom()
    {
        if (dgData.SelectedItem is not RoomInformation room)
            return;

        try
        {
            var hasBookings = _context.BookingDetails.Any(bd => bd.RoomId == room.RoomId);

            if (hasBookings)
            {
                room.RoomStatus = 0;
                _context.RoomInformations.Update(room);
                MessageBox.Show("Room marked as inactive (has booking history).", "Info");
            }
            else
            {
                _context.RoomInformations.Remove(room);
                MessageBox.Show("Room deleted successfully.", "Success");
            }

            _context.SaveChanges();
            LoadRoomData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting room: {ex.Message}", "Error");
        }
    }

    private void DeleteCustomer()
    {
        if (dgData.SelectedItem is not Customer customer)
            return;

        try
        {
            var hasBookings = _context.BookingReservations.Any(br => br.CustomerId == customer.CustomerId);

            if (hasBookings)
            {
                MessageBox.Show("Cannot delete customer with existing bookings.", "Warning");
                return;
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();
            MessageBox.Show("Customer deleted successfully.", "Success");
            LoadCustomerData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting customer: {ex.Message}", "Error");
        }
    }

    private void DeleteOrder()
    {
        if (dgData.SelectedItem is not BookingReservation order)
            return;

        try
        {
            _context.BookingReservations.Remove(order);
            _context.SaveChanges();
            MessageBox.Show("Booking deleted successfully.", "Success");
            LoadOrderData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting booking: {ex.Message}", "Error");
        }
    }

    private void DisplayPage(List<object> data)
    {
        if (data == null || data.Count == 0)
        {
            dgData.ItemsSource = null;
            _totalPages = 1;
            _currentPage = 1;
            UpdatePaginationUI(new List<object>());
            return;
        }

        _totalPages = (int)Math.Ceiling((double)data.Count / ITEMS_PER_PAGE);

        if (_currentPage > _totalPages)
            _currentPage = _totalPages;

        if (_currentPage < 1)
            _currentPage = 1;

        int startIndex = (_currentPage - 1) * ITEMS_PER_PAGE;
        int endIndex = Math.Min(startIndex + ITEMS_PER_PAGE, data.Count);

        _currentPageData = data.GetRange(startIndex, endIndex - startIndex);
        dgData.ItemsSource = _currentPageData;

        UpdatePaginationUI(_currentPageData);
    }

    private void UpdatePaginationUI(List<object> pageData)
    {
        txtPageInfo.Text = $"Page {_currentPage} of {_totalPages}";

        paginationPanel.Children.Clear();

        for (int i = 1; i <= _totalPages; i++)
        {
            int pageNum = i;
            Button pageBtn = new Button
            {
                Content = i.ToString(),
                Width = 36,
                Height = 32,
                Margin = new Thickness(4, 0, 4, 0),
                Background = new SolidColorBrush(i == _currentPage ? System.Windows.Media.Colors.LightBlue : System.Windows.Media.Colors.White),
                Foreground = new SolidColorBrush(System.Windows.Media.Colors.Black),
                BorderThickness = new Thickness(1),
                Tag = pageNum
            };

            pageBtn.Click += (s, e) =>
            {
                _currentPage = pageNum;
                DisplayPage(GetCurrentData());
            };

            paginationPanel.Children.Add(pageBtn);

            if (i >= 5 && _totalPages > 5)
                break;
        }

        btnPrev.IsEnabled = _currentPage > 1;
        btnNext.IsEnabled = _currentPage < _totalPages;
    }

    private void btnPrev_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            DisplayPage(GetCurrentData());
        }
    }

    private void btnNext_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage < _totalPages)
        {
            _currentPage++;
            DisplayPage(GetCurrentData());
        }
    }

    private List<object> GetCurrentData()
    {
        return _currentModule switch
        {
            DashboardModule.Room => _allRooms.Cast<object>().ToList(),
            DashboardModule.Customer => _allCustomers.Cast<object>().ToList(),
            DashboardModule.Order => _allOrders.Cast<object>().ToList(),
            _ => new List<object>()
        };
    }

    private void ResetPagination()
    {
        _currentPage = 1;
        _totalPages = 1;
        _currentPageData.Clear();
    }
}
