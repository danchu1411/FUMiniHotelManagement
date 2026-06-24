using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface IRoomInformationService
    {
        List<RoomInformation> GetAllRooms();
        List<RoomInformation> GetRoomsByStatus(byte status);
        RoomInformation? GetRoomById(int roomId);
        void AddRoom(RoomInformation room);
        void UpdateRoom(RoomInformation room);
        void DeleteRoom(RoomInformation room);
    }
}