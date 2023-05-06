using System.ComponentModel.DataAnnotations.Schema;
using TheRoost.API.Models.Entities;

namespace TheRoost.API.Models.DTOs
{
    public class ReturnRoomDtoWithoutReservations
    {
        public int RoomNumber { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int MaxCapacity { get; set; }
        public int RoomTypeID { get; set; }
        public RoomType? RoomType { get; set; }
    }
}
