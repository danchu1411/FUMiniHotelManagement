using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using BusinessObjects;

namespace ChauTungDangWPF;

public partial class OrderDetailWindow : Window
{
    private readonly BookingReservation _reservation;

    public OrderDetailWindow(BookingReservation reservation)
    {
        InitializeComponent();
        _reservation = reservation;
        LoadData();
    }

    private void LoadData()
    {
        txtReservationId.Text = _reservation.BookingReservationId.ToString(CultureInfo.InvariantCulture);
        txtCustomer.Text = _reservation.Customer?.CustomerFullName ?? _reservation.Customer?.EmailAddress ?? "-";
        txtBookingDate.Text = _reservation.BookingDate?.ToString("yyyy-MM-dd") ?? "-";
        txtStatus.Text = GetStatusText(_reservation.BookingStatus);
        txtTotal.Text = ($"{CalculateTotal():N2}");

        var rows = _reservation.BookingDetails.Select(detail =>
        {
            var nights = Math.Max(1, (detail.EndDate.ToDateTime(TimeOnly.MinValue) - detail.StartDate.ToDateTime(TimeOnly.MinValue)).Days);
            var unitPrice = detail.ActualPrice ?? 0m;
            return new BookingDetailRow
            {
                RoomNumber = detail.Room?.RoomNumber ?? detail.RoomId.ToString(CultureInfo.InvariantCulture),
                RoomTypeName = detail.Room?.RoomType?.RoomTypeName ?? "-",
                StartDate = detail.StartDate.ToString("yyyy-MM-dd"),
                EndDate = detail.EndDate.ToString("yyyy-MM-dd"),
                Nights = nights,
                ActualPrice = unitPrice.ToString("N2"),
                LineTotal = (unitPrice * nights).ToString("N2")
            };
        }).ToList();

        dgDetails.ItemsSource = rows;
    }

    private decimal CalculateTotal()
    {
        return _reservation.BookingDetails.Sum(detail =>
        {
            var nights = Math.Max(1, (detail.EndDate.ToDateTime(TimeOnly.MinValue) - detail.StartDate.ToDateTime(TimeOnly.MinValue)).Days);
            var unitPrice = detail.ActualPrice ?? 0m;
            return unitPrice * nights;
        });
    }

    private static string GetStatusText(byte? status)
    {
        return status switch
        {
            0 => "Cancelled",
            1 => "Booked / Reserved",
            2 => "Checked-In",
            3 => "Checked-Out",
            _ => "Unknown"
        };
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private sealed class BookingDetailRow
    {
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int Nights { get; set; }
        public string ActualPrice { get; set; } = string.Empty;
        public string LineTotal { get; set; } = string.Empty;
    }
}
