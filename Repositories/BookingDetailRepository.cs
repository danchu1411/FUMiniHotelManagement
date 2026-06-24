using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class BookingDetailRepository : IBookingDetailRepository
    {
        public IEnumerable<BookingDetail> GetAllBookingDetails() => BookingDetailDAO.Instance.GetAllBookingDetails();
        public IEnumerable<BookingDetail> GetBookingDetailsByReservationId(int reservationId) => BookingDetailDAO.Instance.GetBookingDetailsByReservationId(reservationId);
        public BookingDetail? GetBookingDetailById(int reservationId, int roomId) => BookingDetailDAO.Instance.GetBookingDetailById(reservationId, roomId);
        public void AddBookingDetails(BookingDetail detail) => BookingDetailDAO.Instance.AddBookingDetails(detail);
        public void UpdateBookingDetails(BookingDetail detail) => BookingDetailDAO.Instance.UpdateBookingDetails(detail);
        public void DeleteBookingDetails(int reservationId, int roomId) => BookingDetailDAO.Instance.DeleteBookingDetails(reservationId, roomId);
        public bool IsRoomOverlapping(int roomId, DateOnly startDate, DateOnly endDate) => BookingDetailDAO.Instance.IsRoomOverlapping(roomId, startDate, endDate);
    }
}
