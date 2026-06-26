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
using Services;

namespace ChauTungDangWPF;

public partial class MainWindow : Window
{
    private readonly bool _isAdmin;
    private readonly string _displayName;
    private readonly FuminiHotelManagementContext _context;
    private readonly IRoomInformationService _roomService;
    private readonly IRoomInformationService _roomInformationService;
    private readonly ICustomerService _customerService;
    private readonly IBookingReservationService _bookingService;
    private readonly IBookingDetailService _bookingDetailService;
    private readonly IRoomTypeService _roomTypeService;

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
        _roomService = new RoomInformationService();
        _customerService = new CustomerService();
        _bookingService = new BookingReservationService();
        _bookingDetailService = new BookingDetailService();
        _roomTypeService = new RoomTypeService();
        _roomInformationService = new RoomInformationService();
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

        gridNormalSearch.Visibility = Visibility.Visible;
        gridReportSearch.Visibility = Visibility.Collapsed;
        gridReportStats.Visibility = Visibility.Collapsed;

        btnCreateRoom.Visibility = Visibility.Collapsed;
        btnCreateRoomType.Visibility = Visibility.Collapsed;
        btnCreateCustomer.Visibility = Visibility.Collapsed;
        btnCreateOrder.Visibility = Visibility.Collapsed;
        btnOrderDetails.Visibility = Visibility.Collapsed;
        btnCheckInOrder.Visibility = Visibility.Collapsed;
        btnCheckOutOrder.Visibility = Visibility.Collapsed;
        btnCancelOrder.Visibility = Visibility.Collapsed;
        btnUpdate.Visibility = Visibility.Collapsed;
        btnDelete.Visibility = Visibility.Collapsed;

        switch (module)
        {
            case DashboardModule.Room:
                txtModuleTitle.Text = "Room Management";
                cboFilter.Items.Add("All");
                cboFilter.Items.Add("Room Number");
                cboFilter.Items.Add("Room Type");
                cboFilter.Items.Add("Status");
                btnCreateRoom.Visibility = Visibility.Visible;
                btnCreateRoomType.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
                LoadRoomData();
                break;

            case DashboardModule.Customer:
                txtModuleTitle.Text = "Customer Management";
                cboFilter.Items.Add("All");
                cboFilter.Items.Add("Customer Name");
                cboFilter.Items.Add("Email");
                cboFilter.Items.Add("Phone Number");
                btnCreateCustomer.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
                LoadCustomerData();
                break;

            case DashboardModule.Order:
                txtModuleTitle.Text = "Order Management";
                cboFilter.Items.Add("All");
                cboFilter.Items.Add("Booking ID");
                cboFilter.Items.Add("Customer Name");
                cboFilter.Items.Add("Status");
                btnOrderDetails.Visibility = Visibility.Visible;
                btnCheckInOrder.Visibility = Visibility.Visible;
                btnCheckOutOrder.Visibility = Visibility.Visible;
                btnCancelOrder.Visibility = Visibility.Visible;
                if (_isAdmin)
                {
                    btnCreateOrder.Visibility = Visibility.Visible;
                }
                else
                {
                    btnCreateOrder.Visibility = Visibility.Collapsed;
                }
                LoadOrderData();
                break;

            case DashboardModule.Report:
                txtModuleTitle.Text = "Report Management";
                gridNormalSearch.Visibility = Visibility.Collapsed;
                gridReportSearch.Visibility = Visibility.Visible;
                gridReportStats.Visibility = Visibility.Visible;
                dpReportStart.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dpReportEnd.SelectedDate = DateTime.Today;
                LoadReportData();
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

            var rawRooms = _roomService.GetAllRooms();
            var allRoomTypes = _roomTypeService.GetAllRoomTypes();

            foreach (var r in rawRooms)
            {
                if (r.RoomType == null)
                {
                    r.RoomType = allRoomTypes.FirstOrDefault(t => t.RoomTypeId == r.RoomTypeId);
                }
            }

            _allRooms = rawRooms;
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

            var rawCustomers = _customerService.GetAllCustomers();

            if (!_isAdmin)
            {
                rawCustomers = rawCustomers.Where(c => c.CustomerFullName == _displayName || c.EmailAddress == _displayName).ToList();
            }

            _allCustomers = rawCustomers;
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

            _allOrders = _bookingService.GetAllBookingReservations().OrderByDescending(b => b.BookingDate).ToList();

            ResetPagination();
            DisplayPage(_allOrders.Cast<object>().ToList());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading orders: {ex.Message}", "Error");
        }
    }

    private void LoadReportData()
    {
        if (dpReportStart.SelectedDate == null || dpReportEnd.SelectedDate == null)
            return;

        DateTime startDateTime = dpReportStart.SelectedDate.Value;
        DateTime endDateTime = dpReportEnd.SelectedDate.Value;

        if (endDateTime < startDateTime)
        {
            MessageBox.Show("End Date must be greater than or equal to Start Date.", "Validation Error");
            return;
        }

        try
        {
            dgData.Columns.Clear();
            dgData.Columns.Add(new DataGridTextColumn { Header = "Booking ID", Binding = new Binding("BookingReservationId"), Width = 100 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Customer", Binding = new Binding("Customer.CustomerFullName"), Width = 150 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Booking Date", Binding = new Binding("BookingDate"), Width = 120 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Total Price", Binding = new Binding("TotalPrice"), Width = 120 });
            dgData.Columns.Add(new DataGridTextColumn { Header = "Status", Binding = new Binding("BookingStatus"), Width = 100 });

            DateOnly startDate = DateOnly.FromDateTime(startDateTime);
            DateOnly endDate = DateOnly.FromDateTime(endDateTime);

            var reportService = new Services.BookingReservationService();
            var reportList = reportService.GetReportByDateRange(startDate, endDate);

            reportList = reportList.OrderByDescending(b => b.BookingDate).ToList();

            decimal totalRevenue = reportList.Where(o => o.BookingStatus != 0).Sum(o => o.TotalPrice ?? 0);
            txtTotalRevenue.Text = $"${totalRevenue:N2}";

            var allDetails = reportList.SelectMany(o => o.BookingDetails).ToList();
            var mostBookedRoomType = allDetails
                .Where(d => d.Room != null && d.Room.RoomType != null)
                .GroupBy(d => d.Room.RoomType.RoomTypeName)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "N/A";
            txtMostBookedRoomType.Text = mostBookedRoomType;

            var topCustomer = reportList
                .Where(o => o.Customer != null)
                .GroupBy(o => o.Customer.CustomerFullName)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "N/A";
            txtTopCustomer.Text = topCustomer;

            _allOrders = reportList;

            ResetPagination();
            DisplayPage(_allOrders.Cast<object>().ToList());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error building statistic report: {ex.Message}", "Error");
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
        if (_currentModule == DashboardModule.Report)
        {
            LoadReportData(); 
        }
        else
        {
            ApplyFilter(); 
        }
    }

    private void ApplyFilter()
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

    private void btnCreateRoom_Click(object sender, RoutedEventArgs e)
    {
        if (_currentModule != DashboardModule.Room)
        {
            MessageBox.Show("Create Room is only available in Room Management.", "Info");
            return;
        }

        var createWindow = new CreateRoomWindow
        {
            Owner = this
        };

        bool? result = createWindow.ShowDialog();
        if (result == true)
        {
            LoadRoomData();
        }
    }

    private void btnCreateRoomType_Click(object sender, RoutedEventArgs e)
    {
        if (_currentModule != DashboardModule.Room)
        {
            MessageBox.Show("Create Room Type is only available in Room Management.", "Info");
            return;
        }

        var createWindow = new CreateRoomTypeWindow
        {
            Owner = this
        };

        bool? result = createWindow.ShowDialog();
        if (result == true)
        {
            LoadRoomData();
        }
    }

    private void btnCreateCustomer_Click(object sender, RoutedEventArgs e)
    {
        if (_currentModule != DashboardModule.Customer)
        {
            MessageBox.Show("Create Customer is only available in Customer Management.", "Info");
            return;
        }

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

    private void btnOrderDetails_Click(object sender, RoutedEventArgs e)
    {
        if (_currentModule != DashboardModule.Order)
        {
            MessageBox.Show("Order details are only available in Order Management.", "Info");
            return;
        }

        var selectedOrder = LoadSelectedOrder();
        if (selectedOrder == null)
        {
            MessageBox.Show("Please select an order to view its details.", "Warning");
            return;
        }

        var detailsWindow = new OrderDetailWindow(selectedOrder)
        {
            Owner = this
        };

        detailsWindow.ShowDialog();
    }

    private void btnCheckInOrder_Click(object sender, RoutedEventArgs e)
    {
        if (_currentModule != DashboardModule.Order)
        {
            MessageBox.Show("Check-in is only available in Order Management.", "Info");
            return;
        }

        var selectedOrder = LoadSelectedOrder();
        if (selectedOrder == null)
        {
            MessageBox.Show("Please select an order to check in.", "Warning");
            return;
        }

        if (selectedOrder.BookingStatus != 1)
        {
            MessageBox.Show("Only booked orders can be checked in.", "Warning");
            return;
        }

        UpdateOrderAndRooms(selectedOrder.BookingReservationId, 2, 2);
        MessageBox.Show("Check-in confirmed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        LoadOrderData();
    }

    private void btnCheckOutOrder_Click(object sender, RoutedEventArgs e)
    {
        if (_currentModule != DashboardModule.Order)
        {
            MessageBox.Show("Check-out is only available in Order Management.", "Info");
            return;
        }

        var selectedOrder = LoadSelectedOrder();
        if (selectedOrder == null)
        {
            MessageBox.Show("Please select an order to check out.", "Warning");
            return;
        }

        if (selectedOrder.BookingStatus != 2)
        {
            MessageBox.Show("Only checked-in orders can be checked out.", "Warning");
            return;
        }

        var invoiceWindow = new CheckoutInvoiceWindow(selectedOrder)
        {
            Owner = this
        };

        bool? result = invoiceWindow.ShowDialog();
        if (result == true)
        {
            UpdateOrderAndRooms(selectedOrder.BookingReservationId, 3, 3, invoiceWindow.InvoiceTotal);
            MessageBox.Show("Check-out completed and invoice confirmed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadOrderData();
        }
    }

    private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
    {
        if (_currentModule != DashboardModule.Order)
        {
            MessageBox.Show("Create Order is only available in Order Management.", "Info");
            return;
        }

        var createWindow = new CreateOrderWindow
        {
            Owner = this
        };

        bool? result = createWindow.ShowDialog();
        if (result == true)
        {
            LoadOrderData(); 
        }
    }

    private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
    {
        if (_currentModule != DashboardModule.Order)
        {
            MessageBox.Show("Cancel is only available in Order Management.", "Info");
            return;
        }

        var selectedOrder = LoadSelectedOrder();
        if (selectedOrder == null)
        {
            MessageBox.Show("Please select an order to cancel.", "Warning");
            return;
        }

        if (selectedOrder.BookingStatus != 1)
        {
            MessageBox.Show("Only booked orders can be cancelled.", "Warning");
            return;
        }

        var confirm = MessageBox.Show("Are you sure you want to cancel this order?", "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes)
            return;

        UpdateOrderAndRooms(selectedOrder.BookingReservationId, 0, 1);
        MessageBox.Show("Order cancelled successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        LoadOrderData();
    }

    private BookingReservation? LoadSelectedOrder()
    {
        if (dgData.SelectedItem is not BookingReservation selected)
            return null;

        using var context = new FuminiHotelManagementContext();
        return context.BookingReservations
            .AsNoTracking()
            .Include(b => b.Customer)
            .Include(b => b.BookingDetails)
                .ThenInclude(d => d.Room)
                    .ThenInclude(r => r.RoomType)
            .FirstOrDefault(b => b.BookingReservationId == selected.BookingReservationId);
    }

    private void UpdateOrderAndRooms(int reservationId, byte bookingStatus, byte roomStatus, decimal? totalPrice = null)
    {
        using var context = new FuminiHotelManagementContext();
        var order = context.BookingReservations
            .Include(b => b.BookingDetails)
                .ThenInclude(d => d.Room)
            .FirstOrDefault(b => b.BookingReservationId == reservationId);

        if (order == null)
        {
            MessageBox.Show("Order not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        order.BookingStatus = bookingStatus;
        if (totalPrice.HasValue)
        {
            order.TotalPrice = totalPrice.Value;
        }

        foreach (var detail in order.BookingDetails)
        {
            if (detail.Room != null)
            {
                detail.Room.RoomStatus = roomStatus;
            }
        }

        context.SaveChanges();
    }

    private void btnLogout_Click(object sender, RoutedEventArgs e)
    {
        var login = new LoginWindow();
        login.Show();
        Close();
    }

    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (_currentModule == DashboardModule.Room)
        {
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
            return;
        }

        if (_currentModule == DashboardModule.Customer)
        {
            if (dgData.SelectedItem is not Customer customer)
            {
                MessageBox.Show("Please select a customer to update.", "Warning");
                return;
            }

            var updateWindow = new UpdateCustomerWindow(customer)
            {
                Owner = this
            };

            bool? result = updateWindow.ShowDialog();
            if (result == true)
            {
                LoadCustomerData();
            }
            return;
        }

        MessageBox.Show("Update functionality is only available for Rooms and Customers.", "Info");
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
            var hasBookings = _bookingService.GetAllBookingReservations()
                .Any(b => b.BookingDetails != null && b.BookingDetails.Any(d => d.RoomId == room.RoomId));

            if (hasBookings)
            {
                room.RoomStatus = 0;
                _roomService.UpdateRoom(room); 
                MessageBox.Show("Room marked as inactive due to existing booking history.", "Info");
            }
            else
            {
                _roomService.DeleteRoom(room); 
                MessageBox.Show("Room deleted successfully.", "Success");
            }

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
            var hasBookings = _bookingService.GetAllBookingReservations()
                .Any(br => br.CustomerId == customer.CustomerId); //

            if (hasBookings)
            {
                MessageBox.Show("Cannot delete customer with existing bookings.", "Warning");
                return;
            }

            _customerService.DeleteCustomer(customer);

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
            _bookingService.DeleteBookingReservation(order);

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
            DashboardModule.Report => _allOrders.Cast<object>().ToList(),
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
