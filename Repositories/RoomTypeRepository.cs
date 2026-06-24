using System.Collections.Generic;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class RoomTypeRepository : IRoomTypeRepository
    {
        public List<RoomType> GetAllRoomTypes() => RoomTypeDAO.Instance.GetAllRoomTypes();
        public RoomType? GetRoomTypeById(int roomTypeId) => RoomTypeDAO.Instance.GetRoomTypeById(roomTypeId);
        public void AddRoomType(RoomType roomType) => RoomTypeDAO.Instance.AddRoomType(roomType);
        public void UpdateRoomType(RoomType roomType) => RoomTypeDAO.Instance.UpdateRoomType(roomType);
        public void DeleteRoomType(RoomType roomType) => RoomTypeDAO.Instance.DeleteRoomType(roomType);
    }
}
