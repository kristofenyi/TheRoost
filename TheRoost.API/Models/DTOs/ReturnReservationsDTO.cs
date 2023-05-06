using TheRoost.API.Models.Entities;

namespace TheRoost.API.Models.DTOs
{
    public class ReturnReservationsDTO
    {
        public Guid ID { get; set; }
        public string AccommodationName { get; set; }
        public string RoomType { get; set; }
        public int RoomNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
    }
}
