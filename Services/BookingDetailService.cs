using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class BookingDetailService : IBookingDetailService
    {
        private readonly IBookingDetailRepository _repo;

        public BookingDetailService()
        {
            _repo = new BookingDetailRepository();
        }

        public IEnumerable<BookingDetail> GetBookingDetailsByReservationId(int reservationId) => _repo.GetBookingDetailsByReservationId(reservationId);

        public void AddBookingDetail(BookingDetail detail, decimal baseRoomPrice)
        {
            // Check Overlap before adding
            if (IsRoomOverlapping(detail.RoomId, detail.StartDate, detail.EndDate))
            {
                throw new Exception("Room is not available during this period. It overlaps with an existing booking.");
            }

            // Calculate actual price based on weekend
            detail.ActualPrice = CalculateDynamicPrice(detail.StartDate, baseRoomPrice);

            _repo.AddBookingDetails(detail);
        }

        public void UpdateBookingDetail(BookingDetail detail) => _repo.UpdateBookingDetails(detail);
        public void DeleteBookingDetails(int reservationId, int roomId) => _repo.DeleteBookingDetails(reservationId, roomId);
        public bool IsRoomOverlapping(int roomId, DateOnly startDate, DateOnly endDate) => _repo.IsRoomOverlapping(roomId, startDate, endDate);

        // Dynamic Weekend Pricing
        public decimal CalculateDynamicPrice(DateOnly startDate, decimal basePrice)
        {
            DayOfWeek dayOfWeek = startDate.DayOfWeek;
            if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
            {
                return basePrice * 1.2m; // 20% increase for weekend
            }
            return basePrice;
        }
    }
}
