using TheRoost.API.Models.Entities;

namespace TheRoost.API.Models.DTOs
{
    public class AccommodationReviewDTO
    {
        public int AccommodationID { get; set; }
        public int Rating { get; set; }
        public string? Text { get; set; }

        public AccommodationReviewDTO()
        {
        }

        public AccommodationReviewDTO(Review review)
        {
            AccommodationID = review.AccommodationID;
            Rating = review.Rating;
            Text = review.Text;
        }
    }
}
