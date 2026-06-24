using System.Collections.Generic;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class RoomInformationRepository : IRoomInformationRepository
    {
        public List<RoomInformation> GetAllRooms() => RoomInformationDAO.Instance.GetAllRooms();
        public List<RoomInformation> GetRoomsByStatus(byte status) => RoomInformationDAO.Instance.GetRoomsByStatus(status);
        public RoomInformation? GetRoomById(int roomId) => RoomInformationDAO.Instance.GetRoomById(roomId);
        public void AddRoom(RoomInformation room) => RoomInformationDAO.Instance.AddRoom(room);
        public void UpdateRoom(RoomInformation room) => RoomInformationDAO.Instance.UpdateRoom(room);
        public void DeleteRoom(RoomInformation room) => RoomInformationDAO.Instance.DeleteRoom(room);
    }
}
