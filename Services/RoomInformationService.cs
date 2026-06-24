using System.Collections.Generic;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class RoomInformationService : IRoomInformationService
    {
        private readonly IRoomInformationRepository _repo;

        public RoomInformationService()
        {
            _repo = new RoomInformationRepository();
        }

        public List<RoomInformation> GetAllRooms() => _repo.GetAllRooms();
        public List<RoomInformation> GetRoomsByStatus(byte status) => _repo.GetRoomsByStatus(status);
        public RoomInformation? GetRoomById(int roomId) => _repo.GetRoomById(roomId);
        public void AddRoom(RoomInformation room) => _repo.AddRoom(room);
        public void UpdateRoom(RoomInformation room) => _repo.UpdateRoom(room);
        public void DeleteRoom(RoomInformation room) => _repo.DeleteRoom(room);
    }
}
