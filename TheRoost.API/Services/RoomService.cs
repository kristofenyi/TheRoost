using Microsoft.EntityFrameworkCore;
using TheRoost.API.AppDbContext;
using TheRoost.API.Models.DTOs;
using AutoMapper;
using TheRoost.API.Models.Entities;
using Microsoft.Extensions.Localization;
using TheRoost.API.Models.Json;

namespace TheRoost.API.Services
{
    public class RoomService
    {
        private readonly MainDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _sharedResources;

        public RoomService(MainDbContext context, IMapper mapper, IStringLocalizer<SharedResource> sharedResources )
        {
            _context = context;
            _mapper = mapper;
            _sharedResources = sharedResources;
        }

        public List<ReturnRoomDTO> ReturnAvailableRooms(DateTime from, DateTime to)
        {
            var availableRooms = _context.Rooms
                .Include(x => x.Reservations)
                .Include(x => x.Accommodation)
                .Include(x => x.RoomType)
                .Where(r => r.Reservations
                .All(res => res.CheckOutDate <= from || res.CheckInDate >= to) || r.Reservations
                .Any(res => res.IsCancelled && res.CheckInDate >= from && res.CheckOutDate <= to))
                .ToList();

            var mappedAvailableRooms = _mapper.Map<List<Room>, List<ReturnRoomDTO>>(availableRooms);
            foreach (var translate in mappedAvailableRooms)
            {
                translate.TypeOfProperty = _sharedResources[translate.TypeOfProperty];
                translate.Description = _sharedResources[translate.Description];
                translate.RoomType = _sharedResources[translate.RoomType];
            }
            return mappedAvailableRooms;
             
        }

        public bool CheckIfDatesAreValid(DateTime from, DateTime to)
        {
            if (from <= to)
                return true;
            else return false;
        }

        public bool IsCorrectDateFormat(string from, string to)
        {
            try
            {
                var fromParsed = DateTime.Parse(from);
                var toParsed = DateTime.Parse(to);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public CheckAvailabilityJson TransfromDatesToJson(string from, string to)
        { 
           var jsonToReturn = new CheckAvailabilityJson();
            jsonToReturn.From = DateTime.Parse(from);
            jsonToReturn.To = DateTime.Parse(to);
            return jsonToReturn;
        }
    }
}
