using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Repositories
{
    public interface IBookingDetailRepository
    {
        IEnumerable<BookingDetail> GetAllBookingDetails();
        IEnumerable<BookingDetail> GetBookingDetailsByReservationId(int reservationId);
        BookingDetail? GetBookingDetailById(int reservationId, int roomId);
        void AddBookingDetails(BookingDetail detail);
        void UpdateBookingDetails(BookingDetail detail);
        void DeleteBookingDetails(int reservationId, int roomId);
        bool IsRoomOverlapping(int roomId, DateOnly startDate, DateOnly endDate);
    }
}
