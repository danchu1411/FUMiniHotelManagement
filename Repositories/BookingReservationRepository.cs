using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class BookingReservationRepository : IBookingReservationRepository
    {
        public List<BookingReservation> GetAllBookingReservations() => BookingReservationDAO.Instance.GetAllBookingReservations();
        public BookingReservation? GetBookingReservationById(int reservationId) => BookingReservationDAO.Instance.GetBookingReservationById(reservationId);
        public List<BookingReservation> GetBookingReservationsByCustomerId(int customerId) => BookingReservationDAO.Instance.GetBookingReservationsByCustomerId(customerId);
        public List<BookingReservation> GetReportByDateRange(DateOnly startDate, DateOnly endDate) => BookingReservationDAO.Instance.GetReportByDateRange(startDate, endDate);
        public void AddBookingReservation(BookingReservation reservation) => BookingReservationDAO.Instance.AddBookingReservation(reservation);
        public void UpdateBookingReservation(BookingReservation reservation) => BookingReservationDAO.Instance.UpdateBookingReservation(reservation);
        public void DeleteBookingReservation(BookingReservation reservation) => BookingReservationDAO.Instance.DeleteBookingReservation(reservation);
    }
}
