using System.Net.Mail;
using System.Net;
using TheRoost.API.AppDbContext;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace TheRoost.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private string baseURL;
        private readonly MainDbContext _context;

        public EmailService(ILogger<EmailService> logger, MainDbContext context)
        {
            _logger = logger;
            baseURL = "https://localhost:7056";
            _context = context;
        }

        public void SendEmail(string to, string subject, string messageBody)
        {
            //SMTP config
            SmtpClient smtp = new SmtpClient("smtp.mailtrap.io", 2525);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("6abea51113c79d", "c60010d235cbaa");
            string from = "messenger@theroost.io";

            MailMessage message = new MailMessage(from, to, subject, messageBody);

            //Add logo before message body
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                $"<img style=\"width:150px;\" src=\"cid:Logo\"><br><br>{messageBody}",
                null,
                "text/html"
            );
            LinkedResource LinkedImage = new LinkedResource("logo.png");
            LinkedImage.ContentId = "Logo";
            htmlView.LinkedResources.Add(LinkedImage);
            message.AlternateViews.Add(htmlView);

            smtp.Send(message);
            _logger.Log(LogLevel.Information, $"Registration confirmation has been sent to {to}.");
        }

        public void SendEmailRegistrationSuccessful(string to) //Email template: Successful registration of a user account
        {
            string subject = "The Roost: Registration completed";
            string messageBody = "Registration has been successful!<br/>You can now log in at The Roost";
            SendEmail(to, subject, messageBody);
        }

        public void SendEmailForgottenPassword(string to,Guid guid)
        {
            string subject = "The Roost: Forgotten password";
            string messageBody = $"Here is link to reset your password<br/>" +
                $" <a href=\"{baseURL}+/GuestAction/get-reset-secret/{guid}\">Reset password</a>";
            SendEmail(to, subject, messageBody);
        }

        public void SendEmailReservationNotification(ReservationDTO reservationDTO)
        {
            var accomodation = _context.Accommodations.FirstOrDefault(x => x.ID == reservationDTO.AccommodationID);
            var managerEmail = _context.Users.FirstOrDefault(x => x.ID == accomodation.AccommodationManagerID).Email;
            var room = _context.Rooms.FirstOrDefault(x => x.ID == reservationDTO.RoomID);
            var roomType = _context.RoomTypes.FirstOrDefault(x => x.ID == room.RoomTypeID);

            if (managerEmail != null)
            {
                string subject = "The Roost: New reservation";
                string messageBody = $"New reservation was created for {accomodation.Name}: <br/>" +
                    $"Room: {room.RoomNumber}, {roomType.Name}<br/>" +
                    $"CheckIn date: {reservationDTO.CheckIn}<br/>" +
                    $"CheckOut date: {reservationDTO.CheckOut}<br/>" +
                    $"Number of guests: {reservationDTO.NumberOfGuests}<br/>" +
                    $"\nHere is link to all reservations in this accommodation: <br/>" +
                    $"<a href=\"{baseURL}/ManagerActions/view-hotel-reservations/{reservationDTO.AccommodationID}\">Reservation notification</a>";
                SendEmail(managerEmail, subject, messageBody);
            }          
        }
    }
}
