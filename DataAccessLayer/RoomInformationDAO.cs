using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class RoomInformationDAO
    {
        private static RoomInformationDAO? instance;
        private static readonly object instanceLock = new object();

        private RoomInformationDAO() { }

        public static RoomInformationDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new RoomInformationDAO();
                    }
                    return instance;
                }
            }
        }

        public List<RoomInformation> GetAllRooms()
        {
            using var context = new FuminiHotelManagementContext();
            return context.RoomInformations
                .Include(r => r.RoomType)
                .ToList();
        }

        public List<RoomInformation> GetRoomsByStatus(byte status)
        {
            using var context = new FuminiHotelManagementContext();
            return context.RoomInformations
                .Include(r => r.RoomType)
                .Where(r => r.RoomStatus == status)
                .ToList();
        }

        public RoomInformation? GetRoomById(int roomId)
        {
            using var context = new FuminiHotelManagementContext();
            return context.RoomInformations
                .Include(r => r.RoomType)
                .FirstOrDefault(r => r.RoomId == roomId);
        }

        public void AddRoom(RoomInformation room)
        {
            using var context = new FuminiHotelManagementContext();
            context.RoomInformations.Add(room);
            context.SaveChanges();
        }

        public void UpdateRoom(RoomInformation room)
        {
            using var context = new FuminiHotelManagementContext();
            context.RoomInformations.Update(room);
            context.SaveChanges();
        }

        public void DeleteRoom(RoomInformation room)
        {
            using var context = new FuminiHotelManagementContext();
            bool hasBookingHistory = context.BookingDetails.Any(b => b.RoomId == room.RoomId);

            if (hasBookingHistory)
            {
                var existingRoom = context.RoomInformations.Find(room.RoomId);
                if (existingRoom != null)
                {
                    existingRoom.RoomStatus = 0;
                    context.SaveChanges();
                }
            }
            else
            {
                context.RoomInformations.Remove(room);
                context.SaveChanges();
            }
        }
    }
}