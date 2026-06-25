using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface IRoomTypeService
    {
        List<RoomType> GetAllRoomTypes();
        RoomType? GetRoomTypeById(int roomTypeId);
        void AddRoomType(RoomType roomType);
        void UpdateRoomType(RoomType roomType);
        void DeleteRoomType(RoomType roomType);
    }
}