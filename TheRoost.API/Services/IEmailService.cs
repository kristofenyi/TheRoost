using TheRoost.API.Models.DTOs;

namespace TheRoost.API.Services
{
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string messageBody);
        void SendEmailRegistrationSuccessful(string to);
        void SendEmailForgottenPassword(string to, Guid guid);
        void SendEmailReservationNotification(ReservationDTO reservationDTO);
    }
}