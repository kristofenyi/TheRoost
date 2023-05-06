using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheRoost.API.Models.Entities
{
    public class Reservation
    {
        [Key]
        public Guid ID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        [ForeignKey("Accommodation")]
        public int AccommodationID { get; set; }
        public Accommodation Accommodations { get; set; }

        [ForeignKey("Room")]
        public int RoomID { get; set; }
        public Room Room { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public bool IsCancelled { get; set; }
    }
}
