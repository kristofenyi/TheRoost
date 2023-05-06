using System.ComponentModel.DataAnnotations.Schema;
using TheRoost.API.Models.Entities;

namespace TheRoost.API.Models.DTOs
{
    public class ReturnRoomDTO
    {
        public string Description { get; set; }
        public double Price { get; set; }
        public int MaxCapacity { get; set; }
        public string AccomodationName { get; set; }
        public string RoomType { get; set; }
        public string TypeOfProperty { get; set; }
    }
}