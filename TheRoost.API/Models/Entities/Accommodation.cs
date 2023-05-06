namespace TheRoost.API.Models.Entities
{
    public class Accommodation
    {
        public int ID { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; } 
        public string City { get; set; }
        public string Region { get; set; }
        public string Description { get; set; }
        public int? StarRating { get; set; }
        public string? TimeZoneName { get; set; }
        public double? UserRating { get; set; }
        public Uri Logo { get; set; }
        public int? AccommodationManagerID { get; set; }
        public AccommodationManager? AccommodationManager { get; set; }
        public List<Room> Rooms { get; set; }
    }
}
