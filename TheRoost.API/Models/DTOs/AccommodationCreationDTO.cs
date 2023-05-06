namespace TheRoost.API.Models.DTOs
{
    public class AccommodationCreationDTO
    {
        public string TypeOfProperty { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Description { get; set; }
        public Uri Logo { get; set; }
        public string TimeZoneName { get; set; }
    }
}
