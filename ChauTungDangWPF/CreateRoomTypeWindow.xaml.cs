using System;
using System.Windows;
using BusinessObjects;
using Services;

namespace ChauTungDangWPF;

public partial class CreateRoomTypeWindow : Window
{
    private readonly IRoomTypeService _roomTypeService;

    public CreateRoomTypeWindow()
    {
        InitializeComponent();
        _roomTypeService = new RoomTypeService();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput())
            return;

        try
        {
            var roomTypeName = txtRoomTypeName.Text.Trim();

            if (_roomTypeService.GetAllRoomTypes().Exists(rt => rt.RoomTypeName != null && rt.RoomTypeName.Equals(roomTypeName, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Room type name already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newRoomType = new RoomType
            {
                RoomTypeName = roomTypeName,
                TypeDescription = txtDescription.Text.Trim(),
                TypeNote = null
            };

            _roomTypeService.AddRoomType(newRoomType);

            MessageBox.Show("Room type created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating room type: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtRoomTypeName.Text))
        {
            MessageBox.Show("Please enter a room type name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
