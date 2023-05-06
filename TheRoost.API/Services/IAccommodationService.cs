using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.Entities.PropertyTypes;

namespace TheRoost.API.Services
{
    public interface IAccommodationService
    {
        void AddAccomodation(Accommodation accommodation);
        ReturnAccommodationDTO GetAccommodationDTOByID(int id);
        bool AccommodationIDExists(int id);
        bool SortAndAddToDB(AccommodationCreationDTO property);
        bool UpdateAccommodation(AccommodationUpdateDTO accommodation);
        void CreateRoom(Room room);
        bool ManagerAuthorization(RoomCreationDTO roomDTO);
        bool RoomValidation(RoomCreationDTO roomDTO);
        bool IsReviewRequestValid(AccommodationReviewDTO reviewDTO, int userID);
        void AddNewReview(AccommodationReviewDTO reviewDTO, int userID);
        List<AccommodationSearchDTO> GetAccommodationsByCityAndGuestCount(string city, int? guestCount);
        bool ValidateUserInputForSearching(string city, int? guestCount);
        List<ReturnAccommodationDTO> GetAccommodationsForUserByID(int userId);
    }
}
