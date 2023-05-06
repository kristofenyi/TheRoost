namespace TheRoost.API.Models.DTOs
{
    public class AccommodationSearchDTO
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? TypeOfProperty { get; set; }
    }
}
