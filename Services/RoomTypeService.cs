using System.Collections.Generic;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IRoomTypeRepository _repo;

        public RoomTypeService()
        {
            _repo = new RoomTypeRepository();
        }

        public List<RoomType> GetAllRoomTypes() => _repo.GetAllRoomTypes();
        public RoomType? GetRoomTypeById(int roomTypeId) => _repo.GetRoomTypeById(roomTypeId);
    }
}
