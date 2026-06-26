using System;
using System.Windows;
using BusinessObjects;
using DataAccessLayer;
using Services;

namespace ChauTungDangWPF;

public partial class CreateRoomWindow : Window
{
    private readonly IRoomInformationService _roomInformationService;
    private readonly IRoomTypeService _roomTypeService;

    public CreateRoomWindow()
    {
        InitializeComponent();
        _context = new FuminiHotelManagementContext();
        _roomInformationService = new RoomInformationService();
        _roomTypeService = new RoomTypeService();
        LoadRoomTypes();
    }

    private void LoadRoomTypes()
    {
        try
        {
            cboRoomType.ItemsSource = _roomTypeService.GetAllRoomTypes();

            cboRoomType.DisplayMemberPath = "RoomTypeName";
            cboRoomType.SelectedValuePath = "RoomTypeId";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading room types: {ex.Message}", "Error");
        }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput())
            return;

        try
        {
            var newRoom = new RoomInformation
            {
                RoomNumber = txtRoomNumber.Text.Trim(),
                RoomDetailDescription = txtDescription.Text.Trim(),
                RoomMaxCapacity = int.Parse(txtCapacity.Text.Trim()),
                RoomPricePerDay = decimal.Parse(txtPrice.Text.Trim()),
                RoomTypeId = (int)cboRoomType.SelectedValue,
                RoomStatus = 1
            };

            _roomInformationService.AddRoom(newRoom);

            MessageBox.Show("Room created successfully!", "Success");
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating room: {ex.Message}", "Error");
        }
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
        {
            MessageBox.Show("Please enter a room number.", "Validation Error");
            return false;
        }

        if (cboRoomType.SelectedItem == null)
        {
            MessageBox.Show("Please select a room type.", "Validation Error");
            return false;
        }

        if (!int.TryParse(txtCapacity.Text, out int capacity) || capacity <= 0)
        {
            MessageBox.Show("Please enter a valid capacity (positive number).", "Validation Error");
            return false;
        }

        if (capacity > 20)
        {
            MessageBox.Show("Max Capacity cannot exceed 20 people.", "Validation Error");
            return false;
        }

        if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
        {
            MessageBox.Show("Please enter a valid price (positive number).", "Validation Error");
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
