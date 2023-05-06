namespace TheRoost.API.Models.Entities
{
    public class Review
    {
        public Guid ID { get; set; }
        public int Rating { get; set; }
        public string? Text { get; set; }
        public DateTime DateCreated { get; set; }

        //FK
        public int UserID { get; set; }
        public User User { get; set; }
        public int AccommodationID { get; set; }
        public Accommodation Accommodation { get; set; }

    }
}
