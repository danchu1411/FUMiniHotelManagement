using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;

namespace DataAccessLayer
{
    public class RoomTypeDAO
    {
        private static RoomTypeDAO? instance = null;
        private static readonly object instanceLock = new object();

        private RoomTypeDAO() { }

        public static RoomTypeDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new RoomTypeDAO();
                    }
                    return instance;
                }
            }
        }

        public List<RoomType> GetAllRoomTypes()
        {
            using var context = new FuminiHotelManagementContext();
            return context.RoomTypes.ToList();
        }

        public RoomType? GetRoomTypeById(int roomTypeId)
        {
            using var context = new FuminiHotelManagementContext();
            return context.RoomTypes.FirstOrDefault(rt => rt.RoomTypeId == roomTypeId);
        }

        public void AddRoomType(RoomType roomType)
        {
            using var context = new FuminiHotelManagementContext();
            context.RoomTypes.Add(roomType);
            context.SaveChanges();
        }

        public void UpdateRoomType(RoomType roomType)
        {
            using var context = new FuminiHotelManagementContext();
            context.RoomTypes.Update(roomType);
            context.SaveChanges();
        }

        public void DeleteRoomType(RoomType roomType)
        {
            using var context = new FuminiHotelManagementContext();
            context.RoomTypes.Remove(roomType);
            context.SaveChanges();
        }
    }
}