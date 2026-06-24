using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Repositories
{
    public interface IBookingReservationRepository
    {
        List<BookingReservation> GetAllBookingReservations();
        BookingReservation? GetBookingReservationById(int reservationId);
        List<BookingReservation> GetBookingReservationsByCustomerId(int customerId);
        List<BookingReservation> GetReportByDateRange(DateOnly startDate, DateOnly endDate);
        void AddBookingReservation(BookingReservation reservation);
        void UpdateBookingReservation(BookingReservation reservation);
        void DeleteBookingReservation(BookingReservation reservation);
    }
}
