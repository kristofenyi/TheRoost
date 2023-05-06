using AutoMapper;
using TheRoost.API.AppDbContext;
using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.Entities.PropertyTypes;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace TheRoost.API.Services
{
    public class AccommodationService : IAccommodationService
    {
        private readonly MainDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _sharedResourceLocalizer;
        private readonly ITranslateService _translateService;

        public AccommodationService(MainDbContext context, IMapper mapper,
            IStringLocalizer<SharedResource> sharedResourceLocalizer,
            ITranslateService translateService)
        {
            _context = context;
            _mapper = mapper;
            _sharedResourceLocalizer = sharedResourceLocalizer;
            _translateService = translateService;
        }

        public void AddAccomodation(Accommodation accommodation)
        {
            //Translation of description
            var translation = _translateService.TranslateToCZandSK(accommodation.Description);
            _translateService.WriteToResourceFileSK(accommodation.Description, translation.translations.FirstOrDefault(x => x.to == "sk").text);
            _translateService.WriteToResourceFileCS(accommodation.Description, translation.translations.FirstOrDefault(x => x.to == "cs").text);
            _context.Add(accommodation);
            _context.SaveChanges();
        }

        public bool UpdateAccommodation(AccommodationUpdateDTO accommodation)
        {
            // Updates only those properties which were provided (= not null) by the user
            var currentAccommodation = _context.Accommodations.FirstOrDefault(x => x.ID == accommodation.ID);
            if (currentAccommodation == null)
                return false;
            //descrion is no null
            if (accommodation.Description != null)
            {
                //Translation
                var translation = _translateService.TranslateToCZandSK(accommodation.Description);
                _translateService.WriteToResourceFileSK(accommodation.Description, translation.translations.FirstOrDefault(x => x.to == "sk").text);
                _translateService.WriteToResourceFileCS(accommodation.Description, translation.translations.FirstOrDefault(x => x.to == "cs").text);
            }
            _mapper.Map(accommodation, currentAccommodation); // update not null (Automapper)
            _context.SaveChanges();
            return true;
        }

        public bool SortAndAddToDB(AccommodationCreationDTO property)
        {
            //TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(property.TimeZoneName);
            //if (timeZoneInfo == null)

            var timeZoneInfo = TimeZoneInfo.GetSystemTimeZones()
            .FirstOrDefault(tz => tz.Id == property.TimeZoneName);

            if (timeZoneInfo != null)
            {
                switch (property.TypeOfProperty.ToLower())
                {
                    case "hotel":
                        var mappedToHotel = _mapper.Map<Hotel>(property);
                        AddAccomodation(mappedToHotel);
                        return true;
                    case "hostel":
                        AddAccomodation(_mapper.Map<Hostel>(property));
                        return true;
                    case "apartment":
                        AddAccomodation(_mapper.Map<Apartment>(property));
                        return true;
                    case "guesthouse":
                        AddAccomodation(_mapper.Map<Guesthouse>(property));
                        return true;
                    default:
                        return false;
                }
            }
            else return false;
        }

        public bool AccommodationIDExists(int id)
        {
            return _context.Accommodations.Any(x => x.ID == id);
        }

        public ReturnAccommodationDTO GetAccommodationDTOByID(int id)
        {
            var thisAccommodation = _context.Accommodations.FirstOrDefault(x => x.ID == id);
            var mappedAccommodation = _mapper.Map<ReturnAccommodationDTO>(thisAccommodation);
            //Return description & TypeOfProperty based on localization
            mappedAccommodation.Description = _sharedResourceLocalizer[mappedAccommodation.Description];
            mappedAccommodation.TypeOfProperty = _sharedResourceLocalizer[thisAccommodation.GetType().Name];
            return mappedAccommodation;
        }

        public List<AccommodationSearchDTO> GetAccommodationsByCityAndGuestCount(string city, int? guestCount)
        {
            List<Accommodation> searchResults;
            if (guestCount is null)
            {
                searchResults = _context.Accommodations
                    .Where(a => a.City.ToUpper().Contains(city.ToUpper()))
                    .OrderBy(a => a.Name)
                    .ToList();
            } else
            {
                searchResults = _context.Accommodations
                    .Where(a => a.City.ToUpper().Contains(city.ToUpper()) &&
                    a.Rooms.Sum(r => r.MaxCapacity) >= guestCount)
                    .ToList();
            }

            //Convert to DTO
            return _mapper.Map<List<AccommodationSearchDTO>>(searchResults);
        }

        public bool ValidateUserInputForSearching(string city, int? guestCount) =>
            (guestCount is null || guestCount > 0 && guestCount <= 200) &&
            city.Length >= 2 && city.Length < 30;
        
        public void CreateRoom(Room room)
        {
            //Transalte Description
            var translation = _translateService.TranslateToCZandSK(room.Description);
            _translateService.WriteToResourceFileSK(room.Description, translation.translations.FirstOrDefault(x => x.to == "sk").text);
            _translateService.WriteToResourceFileCS(room.Description, translation.translations.FirstOrDefault(x => x.to == "cs").text);
            _context.Rooms.Add(room);
            _context.SaveChanges();
        }

        public bool ManagerAuthorization(RoomCreationDTO roomDTO)
        {
            var managerID = roomDTO.Rooms.FirstOrDefault().ManagerID;
            int[] managerAccomodations = _context
                                        .Accommodations
                                        .Where(x => x.AccommodationManagerID == managerID)
                                        .Select(x => x.ID)
                                        .ToArray();

            return roomDTO.Rooms.All(room => managerAccomodations.Contains(room.AccommodationID));
        }

        public bool RoomValidation(RoomCreationDTO roomDTO)
        {
            Dictionary<int, int> hotelRooms = new Dictionary<int, int>();

            foreach (var room in roomDTO.Rooms)
            {
                if (!_context
                    .Rooms
                    .Where(x => x.AccommodationID == room.AccommodationID && x.RoomNumber == room.RoomNumber)
                    .IsNullOrEmpty())
                    return false;
            }
            return true;
        }

        public bool IsReviewRequestValid(AccommodationReviewDTO reviewDTO, int userID)
        {
            //Check if user stayed in the accommodation and has a concluded (and not-cancelled) reservation in it
            bool hasConcludedReservation = _context.Reservations
                .Where(r => r.UserID == userID && r.AccommodationID == reviewDTO.AccommodationID)
                .Any(r => r.CheckOutDate.Date <= DateTime.Now.Date && r.IsCancelled == false);

            //Check if user has already reviewed this accommodation
            bool hasNotReviewedYet = _context.Reviews
                .Where(r => r.UserID == userID && r.AccommodationID == reviewDTO.AccommodationID)
                .IsNullOrEmpty();

            //Check if above is satisfied plus if rating is above or equal to 1 star and below or equal to 5 stars
            if (hasConcludedReservation && hasNotReviewedYet && reviewDTO.Rating >= 1 && reviewDTO.Rating <= 5)
                return true;
            else
                return false;

        }

        public void AddNewReview(AccommodationReviewDTO reviewDTO, int userID)
        {
            _context.Reviews.Add(new Review
            {
                AccommodationID = reviewDTO.AccommodationID,
                UserID = userID,
                DateCreated = DateTime.Now.Date,
                Rating = reviewDTO.Rating,
                Text = reviewDTO.Text
            });
            _context.SaveChanges();
            RecountAccommodationRating(reviewDTO.AccommodationID);
        }

        public void RecountAccommodationRating(int accommodationID)
        {
            //Compute average rating
            double averageRating = _context.Reviews
                .Where(a => a.AccommodationID == accommodationID)
                .Average(a => a.Rating);

            //Adjust rating in model
            _context.Accommodations
                .FirstOrDefault(a => a.ID == accommodationID).UserRating = averageRating;

            var success = _context.SaveChanges();
            Console.WriteLine();
        }

        public List<ReturnAccommodationDTO> GetAccommodationsForUserByID(int userId)
        {
            var accommodations = _context.Accommodations.Where(a => a.AccommodationManagerID == userId).ToList();
            return _mapper.Map<List<Accommodation>, List<ReturnAccommodationDTO>>(accommodations);
        }
    }
}
