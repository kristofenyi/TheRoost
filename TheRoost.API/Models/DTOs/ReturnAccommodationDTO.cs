using System.ComponentModel.DataAnnotations.Schema;
using TheRoost.API.Models.Entities;

namespace TheRoost.API.Models.DTOs
{
    public class ReturnAccommodationDTO
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
        public int FloorNumber { get; set; }
        public int? NumberOfFloors { get; set; }
        public int? UserRating { get; set; }
        public int? StarRating { get; set; }
        public List<Room> Rooms { get; set; }
        public string? TimeZoneName { get; set; }
        public TimeZoneInfo TimeZone
        {
            get
                {
                 return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
                }
        }
    }
}
