using System;
using System.Collections.Generic;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class BookingReservationService : IBookingReservationService
    {
        private readonly IBookingReservationRepository _repo;

        public BookingReservationService()
        {
            _repo = new BookingReservationRepository();
        }

        public List<BookingReservation> GetAllBookingReservations() => _repo.GetAllBookingReservations();
        public BookingReservation? GetBookingReservationById(int reservationId) => _repo.GetBookingReservationById(reservationId);
        public List<BookingReservation> GetBookingReservationsByCustomerId(int customerId) => _repo.GetBookingReservationsByCustomerId(customerId);
        public List<BookingReservation> GetReportByDateRange(DateOnly startDate, DateOnly endDate) => _repo.GetReportByDateRange(startDate, endDate);
        public void AddBookingReservation(BookingReservation reservation) => _repo.AddBookingReservation(reservation);
        public void UpdateBookingReservation(BookingReservation reservation) => _repo.UpdateBookingReservation(reservation);
        public void DeleteBookingReservation(BookingReservation reservation) => _repo.DeleteBookingReservation(reservation);
    }
}
