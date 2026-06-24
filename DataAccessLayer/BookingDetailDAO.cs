using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class BookingDetailDAO
    {
        private static BookingDetailDAO? _instance;
        private static readonly object _instanceLock = new object();

        private BookingDetailDAO() { }

        public static BookingDetailDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if(_instance == null)
                    {
                        _instance = new BookingDetailDAO();
                    }
                    return _instance;
                }
            }
        }

        public IEnumerable<BookingDetail> GetAllBookingDetails()
        {
            using var context = new FuminiHotelManagementContext();
            return context.BookingDetails
                .Include(b => b.Room)
                .Include(b => b.BookingReservation)
                .ToList();
        }

        public IEnumerable<BookingDetail> GetBookingDetailsByReservationId(int reservationId)
        {
            using var context = new FuminiHotelManagementContext();
            return context.BookingDetails
                .Include(b => b.Room)
                .Where(b => b.BookingReservationId == reservationId);
        }

        public BookingDetail? GetBookingDetailById(int reservationId, int roomId)
        {
            using var context = new FuminiHotelManagementContext();
            return context.BookingDetails
                .Include(b => b.Room)
                .Include(b => b.BookingReservation)
                .FirstOrDefault(b => b.BookingReservationId == reservationId && b.RoomId == roomId);
        }

        public void AddBookingDetails(BookingDetail detail)
        {
            using var context = new FuminiHotelManagementContext();
            context.BookingDetails.Add(detail);
            context.SaveChanges();
        }

        public void UpdateBookingDetails(BookingDetail detail)
        {
            using var context = new FuminiHotelManagementContext();
            context.BookingDetails.Update(detail);
            context.SaveChanges();
        }

        public void DeleteBookingDetails(int reservationId, int roomId)
        {
            using var context = new FuminiHotelManagementContext();
            var detail = context.BookingDetails.FirstOrDefault(b => b.BookingReservationId == reservationId && b.RoomId == roomId);
            if(detail != null)
            {
                context.BookingDetails.Remove(detail);
                context.SaveChanges();
            }
        }

        public bool IsRoomOverlapping(int roomId, DateOnly startDate, DateOnly endDate)
        {
            using var context = new FuminiHotelManagementContext();
            return context.BookingDetails
                .Include(b => b.BookingReservation)
                .Any(b => b.RoomId == roomId
                   && b.BookingReservation.BookingStatus == 1 || b.BookingReservation.BookingStatus == 2
                   && b.StartDate < endDate
                   && b.EndDate > startDate);
        }
    }
}

