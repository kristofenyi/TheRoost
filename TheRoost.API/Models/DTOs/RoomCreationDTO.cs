using TheRoost.API.Models.Entities;

namespace TheRoost.API.Models.DTOs
{
    public class RoomCreationDTO
    {
        public List<Room> Rooms { get; set; }

        public RoomCreationDTO()
        {
        }

        public RoomCreationDTO(List<Room> rooms)
        {
            Rooms = rooms;
        }
    }
}
