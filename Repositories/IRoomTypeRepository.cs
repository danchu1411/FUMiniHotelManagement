using System.Collections.Generic;
using BusinessObjects;

namespace Repositories
{
    public interface IRoomTypeRepository
    {
        List<RoomType> GetAllRoomTypes();
        RoomType? GetRoomTypeById(int roomTypeId);
        void AddRoomType(RoomType roomType);
        void UpdateRoomType(RoomType roomType);
        void DeleteRoomType(RoomType roomType);
    }
}
