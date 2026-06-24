using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface IBookingDetailService
    {
        IEnumerable<BookingDetail> GetBookingDetailsByReservationId(int reservationId);
        void AddBookingDetail(BookingDetail detail, decimal baseRoomPrice);
        void UpdateBookingDetail(BookingDetail detail);
        void DeleteBookingDetails(int reservationId, int roomId);
        bool IsRoomOverlapping(int roomId, DateOnly startDate, DateOnly endDate);
        decimal CalculateDynamicPrice(DateOnly startDate, decimal basePrice);
    }
}