using System.ComponentModel.DataAnnotations.Schema;

namespace TheRoost.API.Models.Entities
{
    public class Room
    {
        public int ID { get; set; }
        public int RoomNumber { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int MaxCapacity { get; set; }
        public List<Reservation>? Reservations { get; set; }
        [NotMapped]
        public int ManagerID { get; set; }

        //FK
        public int AccommodationID { get; set; }
        public Accommodation? Accommodation { get; set; }
        public int RoomTypeID { get; set; }
        public RoomType? RoomType { get; set; }
        // public RoomAmenities RoomAmenities { get; set; }

        public Room()
        {
        }
        public Room(int roomNumber, string description, double price, int maxCapacity, int accommodationID, int roomTypeID)
        {
            RoomNumber = roomNumber;
            Description = description;
            Price = price;
            MaxCapacity = maxCapacity;
            AccommodationID = accommodationID;
            RoomTypeID = roomTypeID;
        }
        //constructor for seeding
        public Room(int ID, int roomNumber, string description, double price, int maxCapacity, int accommodationID, int roomTypeID)
        {
            ID = ID;
            RoomNumber = roomNumber;
            Description = description;
            Price = price;
            MaxCapacity = maxCapacity;
            AccommodationID = accommodationID;
            RoomTypeID = roomTypeID;
        }

    }
}
