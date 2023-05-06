using TheRoost.API.Models.Entities;

namespace TheRoost.API.Models.DTOs
{
    public class ReservationDTO
    {
        public int AccommodationID { get; set; }
        public int RoomID { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NumberOfGuests { get; set; }
        public bool IsCancelled { get; set; }
    }
}
