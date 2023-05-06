using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TheRoost.API.Models.Entities;

namespace TheRoost.API.Models.DTOs
{
    public class ReturnUserReservation
    {
        public Guid ID { get; set; }
        public int AccommodationID { get; set; }
        public Accommodation Accommodations { get; set; }
        public ReturnRoomDtoWithoutReservations Room { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
    }
}
