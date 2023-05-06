namespace TheRoost.API.Models.Entities
{
    public class RoomAmenities
    {
        public int ID { get; set; }
        public bool HasPool { get; set; }
        public bool HasSpa { get; set; }
        public bool HasSafe { get; set; }
        public bool HasGym { get; set; }
        public bool HandicapAccessible { get; set; }
    }
}
