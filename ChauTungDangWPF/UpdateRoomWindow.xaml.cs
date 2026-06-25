using System;
using System.Linq;
using System.Windows;
using BusinessObjects;
using DataAccessLayer;

namespace ChauTungDangWPF;

public partial class UpdateRoomWindow : Window
{
    private readonly FuminiHotelManagementContext _context;
    private readonly RoomInformation _room;

    public UpdateRoomWindow(RoomInformation room)
    {
        InitializeComponent();
        _context = new FuminiHotelManagementContext();
        _room = room;
        LoadRoomTypes();
        LoadRoomData();
    }

    private void LoadRoomTypes()
    {
        try
        {
            cboRoomType.ItemsSource = _context.RoomTypes.ToList();
            cboRoomType.DisplayMemberPath = "RoomTypeName";
            cboRoomType.SelectedValuePath = "RoomTypeId";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading room types: {ex.Message}", "Error");
        }
    }

    private void LoadRoomData()
    {
        txtRoomNumber.Text = _room.RoomNumber;
        txtDescription.Text = _room.RoomDetailDescription;
        txtCapacity.Text = _room.RoomMaxCapacity?.ToString() ?? string.Empty;
        txtPrice.Text = _room.RoomPricePerDay?.ToString() ?? string.Empty;
        cboRoomType.SelectedValue = _room.RoomTypeId;
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput())
            return;

        try
        {
            var roomNumber = txtRoomNumber.Text.Trim();

            var duplicateRoom = _context.RoomInformations.FirstOrDefault(r =>
                r.RoomId != _room.RoomId &&
                r.RoomNumber != null &&
                r.RoomNumber.ToLower() == roomNumber.ToLower());

            if (duplicateRoom != null)
            {
                MessageBox.Show("Room number already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existingRoom = _context.RoomInformations.FirstOrDefault(r => r.RoomId == _room.RoomId);
            if (existingRoom == null)
            {
                MessageBox.Show("Room not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            existingRoom.RoomNumber = roomNumber;
            existingRoom.RoomDetailDescription = txtDescription.Text.Trim();
            existingRoom.RoomMaxCapacity = int.Parse(txtCapacity.Text.Trim());
            existingRoom.RoomPricePerDay = decimal.Parse(txtPrice.Text.Trim());
            existingRoom.RoomTypeId = (int)cboRoomType.SelectedValue;

            _context.SaveChanges();

            MessageBox.Show("Room updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
        {
            MessageBox.Show("Please enter a room number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (cboRoomType.SelectedItem == null)
        {
            MessageBox.Show("Please select a room type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (!int.TryParse(txtCapacity.Text, out int capacity) || capacity <= 0)
        {
            MessageBox.Show("Please enter a valid capacity (positive number).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (capacity > 20)
        {
            MessageBox.Show("Max Capacity cannot exceed 20 people.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
        {
            MessageBox.Show("Please enter a valid price (positive number).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
