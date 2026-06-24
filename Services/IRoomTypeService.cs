using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface IRoomTypeService
    {
        List<RoomType> GetAllRoomTypes();
        RoomType? GetRoomTypeById(int roomTypeId);
    }
}