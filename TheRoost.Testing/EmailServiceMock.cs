using System.Net.Mail;
using System.Net;
using TheRoost.API.Services;
using TheRoost.API.Models.DTOs;

namespace TheRoost.Testing
{
    public class EmailServiceMock : IEmailService
    {

        public EmailServiceMock()
        {
        }

        public void SendEmail(string to, string subject, string messageBody) { }
        public void SendEmailRegistrationSuccessful(string to) { }
        public void SendEmailForgottenPassword(string to, Guid guid) { }
        public void SendEmailReservationNotification(ReservationDTO reservationDTO) { }
    }
}
