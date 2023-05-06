using TheRoost.API.Models.DTOs;

namespace TheRoost.API.Services
{
    public interface IManagerActionService
    {
        List<ReturnReservationsDTO> GetReservationsForManager(int managerID);
        List<ReturnReservationsDTO> GetReservationsForHotel(int accommodationID);
        bool ValidateManagerAccommodation(int managerID, int accommodationID);
    }
}