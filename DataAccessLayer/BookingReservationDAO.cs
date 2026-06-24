using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class BookingReservationDAO
    {
        private static BookingReservationDAO? instance = null;
        private static readonly object instanceLock = new object();

        private BookingReservationDAO() { }

        public static BookingReservationDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new BookingReservationDAO();
                    }
                    return instance;
                }
            }
        }

        public List<BookingReservation> GetAllBookingReservations()
        {
            using var context = new FuminiHotelManagementContext();
            return context.BookingReservations
                .Include(b => b.Customer)
                .ToList();
        }

        public BookingReservation? GetBookingReservationById(int reservationId)
        {
            using var context = new FuminiHotelManagementContext();
            return context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Room)
                .FirstOrDefault(b => b.BookingReservationId == reservationId);
        }

        public List<BookingReservation> GetBookingReservationsByCustomerId(int customerId)
        {
            using var context = new FuminiHotelManagementContext();
            return context.BookingReservations
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }

        public void AddBookingReservation(BookingReservation reservation)
        {
            using var context = new FuminiHotelManagementContext();
            context.BookingReservations.Add(reservation);
            context.SaveChanges();
        }

        public void UpdateBookingReservation(BookingReservation reservation)
        {
            using var context = new FuminiHotelManagementContext();
            context.BookingReservations.Update(reservation);
            context.SaveChanges();
        }

        public void DeleteBookingReservation(BookingReservation reservation)
        {
            using var context = new FuminiHotelManagementContext();
            context.BookingReservations.Remove(reservation);
            context.SaveChanges();
        }

        public List<BookingReservation> GetReportByDateRange(DateOnly startDate, DateOnly endDate)
        {
            using var context = new FuminiHotelManagementContext();
            return context.BookingReservations
                .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate)
                .OrderByDescending(b => b.BookingDate) 
                .Include(b => b.Customer)
                .ToList();
        }
    }
}