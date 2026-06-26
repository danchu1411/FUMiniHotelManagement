using System;
using System.Linq;
using System.Windows;
using BusinessObjects;
using Services;

namespace ChauTungDangWPF;

public partial class CreateOrderWindow : Window
{
    private readonly ICustomerService _customerService;
    private readonly IRoomInformationService _roomService;
    private readonly bool _isAdmin;
    private readonly string _displayName;
    private readonly IBookingReservationService _reservationService;
    private readonly IBookingDetailService _bookingDetailService;

    public CreateOrderWindow(bool isAdmin, string displayName)
    {
        InitializeComponent();

        _customerService = new CustomerService();
        _roomService = new RoomInformationService();
        _reservationService = new BookingReservationService();
        _bookingDetailService = new BookingDetailService();
        _isAdmin = isAdmin;
        _displayName = displayName;

        LoadInitData();
    }

    private void LoadInitData()
    {
        try
        {
            var customers = _customerService.GetAllCustomers();
            cboCustomer.ItemsSource = customers;
            cboCustomer.DisplayMemberPath = "CustomerFullName";
            cboCustomer.SelectedValuePath = "CustomerId";

            if (!_isAdmin)
            {
                var currentCustomer = customers.FirstOrDefault(c =>
                    c.CustomerFullName == _displayName || c.EmailAddress == _displayName);

                if (currentCustomer != null)
                {
                    cboCustomer.SelectedValue = currentCustomer.CustomerId;
                    cboCustomer.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Could not sync your customer profile data. Action denied.", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }
            }

            cboRoom.ItemsSource = _roomService.GetAllRooms();
            cboRoom.DisplayMemberPath = "RoomNumber";
            cboRoom.SelectedValuePath = "RoomId";

            dpStartDate.SelectedDate = DateTime.Today;
            dpEndDate.SelectedDate = DateTime.Today.AddDays(1);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading initialization data: {ex.Message}", "Error");
        }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput())
            return;

        try
        {
            int selectedCustomerId = (int)cboCustomer.SelectedValue;
            int selectedRoomId = (int)cboRoom.SelectedValue;

            DateOnly startDate = DateOnly.FromDateTime(dpStartDate.SelectedDate!.Value);
            DateOnly endDate = DateOnly.FromDateTime(dpEndDate.SelectedDate!.Value);

            if (_bookingDetailService.IsRoomOverlapping(selectedRoomId, startDate, endDate))
            {
                MessageBox.Show("This room is already reserved or occupied during the selected period. Please choose another date or room.", "Overlapping Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var room = _roomService.GetRoomById(selectedRoomId);
            if (room == null)
            {
                MessageBox.Show("Selected room data not found.", "Error");
                return;
            }

            decimal basePrice = room.RoomPricePerDay ?? 0;
            decimal actualPrice = _bookingDetailService.CalculateDynamicPrice(startDate, basePrice);

            int totalDays = (dpEndDate.SelectedDate.Value - dpStartDate.SelectedDate.Value).Days;
            decimal totalOrderPrice = actualPrice * totalDays;

            int nextBookingId = (_reservationService.GetAllBookingReservations().Any()
                ? _reservationService.GetAllBookingReservations().Max(b => b.BookingReservationId)
                : 0) + 1;

            var newReservation = new BookingReservation
            {
                BookingReservationId = nextBookingId,
                BookingDate = DateOnly.FromDateTime(DateTime.Today),
                TotalPrice = totalOrderPrice,
                CustomerId = selectedCustomerId,
                BookingStatus = 1
            };

            var newDetail = new BookingDetail
            {
                BookingReservationId = nextBookingId,
                RoomId = selectedRoomId,
                StartDate = startDate,
                EndDate = endDate,
                ActualPrice = actualPrice
            };

            _reservationService.AddBookingReservation(newReservation);
            _bookingDetailService.AddBookingDetail(newDetail, basePrice);

            MessageBox.Show($"Booking order created successfully!\nTotal Days: {totalDays} days.\nPrice/Day applied: ${actualPrice:N2}\nTotal Price: ${totalOrderPrice:N2}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating booking order: {ex.Message}", "Error");
        }
    }

    private bool ValidateInput()
    {
        if (cboCustomer.SelectedItem == null)
        {
            MessageBox.Show("Please select a customer.", "Validation Error");
            return false;
        }

        if (cboRoom.SelectedItem == null)
        {
            MessageBox.Show("Please select a room.", "Validation Error");
            return false;
        }

        if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
        {
            MessageBox.Show("Please select both check-in and check-out dates.", "Validation Error");
            return false;
        }

        DateTime startDate = dpStartDate.SelectedDate.Value;
        DateTime endDate = dpEndDate.SelectedDate.Value;

        if (startDate < DateTime.Today)
        {
            MessageBox.Show("Check-in date cannot be in the past.", "Validation Error");
            return false;
        }

        if (endDate <= startDate)
        {
            MessageBox.Show("Check-out date must be greater than Check-in date.", "Validation Error");
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