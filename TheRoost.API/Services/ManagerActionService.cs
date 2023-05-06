using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using TheRoost.API.AppDbContext;
using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.Entities;

namespace TheRoost.API.Services
{
    public class ManagerActionService : IManagerActionService
    {
        private readonly MainDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _sharedResources;

        public ManagerActionService(MainDbContext context, IMapper mapper,
            IStringLocalizer<SharedResource> sharedResources,
            ITranslateService translateService)
        {
            _context = context;
            _mapper = mapper;
            _sharedResources = sharedResources;
        }
        public List<ReturnReservationsDTO> GetReservationsForManager(int managerID)
        {
            var upcommingReservations = _context.Reservations
                .Include(x => x.Accommodations)
                .Include(z => z.Room)
                .Include(y => y.Room.RoomType)
                .Where(x => x.Accommodations.AccommodationManagerID == managerID)
                .Where(x => x.IsCancelled == false)
                .Where(x => x.CheckInDate > DateTime.UtcNow)
                .OrderBy(x => x.AccommodationID).ThenBy(x => x.CheckInDate)
                .ToList();
            var mappedAvailableRooms = _mapper.Map<List<Reservation>, List<ReturnReservationsDTO>>(upcommingReservations);

            return mappedAvailableRooms;
        }
        public List<ReturnReservationsDTO> GetReservationsForHotel(int accommodationID)
        {
            var hotelReservations = _context.Reservations
                .Include(x => x.Accommodations)
                .Include(z => z.Room)
                .Include(y => y.Room.RoomType)
                .Where(x => x.Accommodations.ID == accommodationID)
                .Where(x => x.IsCancelled == false)
                .Where(x => x.CheckInDate > DateTime.UtcNow)
                .OrderBy(x => x.AccommodationID).ThenBy(x => x.CheckInDate)
                .ToList();
            var mappedHotelReservations = _mapper.Map<List<Reservation>, List<ReturnReservationsDTO>>(hotelReservations);

            return mappedHotelReservations;
        }

        public bool ValidateManagerAccommodation(int managerID, int accommodationID)
        {
            var accommodationManager = _context
                                        .Accommodations
                                        .FirstOrDefault(x => x.ID == accommodationID)
                                        .AccommodationManagerID;
            if (accommodationManager == managerID)
                return true;
            else
                return false;
        }
    }
}
