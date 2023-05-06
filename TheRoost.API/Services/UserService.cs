using Microsoft.AspNetCore.Components.Forms;
using System.Security.Cryptography;
using System.Text;
using TheRoost.API.AppDbContext;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.DTOs;
using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace TheRoost.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MainDbContext _context;
        private readonly IMapper _mapper;

        public UserService(MainDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
        }

        public string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var asBytes = Encoding.Default.GetBytes(password);
            var hashed = sha.ComputeHash(asBytes);
            return Convert.ToBase64String(hashed);
        }

        public void RegisterNewUser(UserRegistrationDTO user)
        {
            var rand = new Random();
            var salt = rand.Next();
            var hashedPassword = HashPassword($"{user.Password}{salt}");
            var newUser = new User()
            {
                Email = user.Email,
                Password = hashedPassword,
                Salt = salt,
                Name = "user",
                RoleID = 2
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            newUser.Name = String.Concat(newUser.Name,_context.Users.FirstOrDefault(x => x.Email == newUser.Email)?.ID);
            _context.SaveChanges();
        }

        public bool EmailInputValidation(string email)
        {
            //TODO check if string containts @ and "."
            if (email.Length > 6 && email.Length < 20)
            {
                return true;
            } 
            return false;
        }

        public bool UserInputValidation(string password, string passwordConfirmation)
        {
            if ((password.Length < 6 || password.Length > 20)) return false;
            if (!(password.Equals(passwordConfirmation))) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsDigit)) return false;
            if (!password.All(char.IsLetterOrDigit)) return false;
            return true;           
        }

        public User Login(UserLoginDTO userLogin)
        {
            var thisUser = _context.Users.FirstOrDefault(u => u.Email == userLogin.Email);
            var tryToUsePasswrod = HashPassword($"{userLogin.Password}{thisUser.Salt}");
            if (tryToUsePasswrod == thisUser.Password)
            {
                return thisUser;
            }
            return null;         
        }
        public User GetUserByIDClaim()
        {
            var user = _context
                .Users.FirstOrDefault(u => u.ID == int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return user;
        }
        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public bool CheckIfUserExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }


        public Reservation GetReservatoionByID(Guid guid)
        {
            return _context.Reservations.FirstOrDefault(x => x.ID == guid);
        }
        public bool CancelReservation(Guid guid)
        {
            var reservation = GetReservatoionByID(guid);            

            if (reservation != null)
            {
                var accomodationTimeZone = _context.Accommodations.FirstOrDefault(x => x.ID == reservation.AccommodationID).TimeZoneName;
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(accomodationTimeZone);
                var timeZoneOffSet = timeZoneInfo.BaseUtcOffset.Hours;

                if (reservation.CheckInDate.AddHours(timeZoneOffSet) > DateTime.UtcNow.AddHours(24))
                {
                    reservation.IsCancelled = true;
                    _context.SaveChanges();
                    return true;
                } return false;                
            } return false;
        }

        public string GetUserEmailClaim()
        {
            return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        }

        public int GetUserIDClaim()
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        public bool EditUserAccount(AccountEditDTO accountEditDTO)
        {
           var user = _context.Users.FirstOrDefault(x => x.ID == accountEditDTO.ID);
           var updatePassword = String.Empty;
            if (user != null)
            {
                if ((accountEditDTO.Email != null) && (accountEditDTO.Email.Length < 6 || accountEditDTO.Email.Length > 20)) return false; 

                if (accountEditDTO.OldPassword != null)
                {
                    if (user.Password != HashPassword($"{accountEditDTO.OldPassword}{user.Salt}")) return false;
                    if (accountEditDTO.NewPassword.Length < 6 || accountEditDTO.NewPassword.Length > 20) return false;
                    if (!(accountEditDTO.NewPassword.Equals(accountEditDTO.PasswordConfirmation))) return false;
                    if (!accountEditDTO.NewPassword.Any(char.IsUpper)) return false;
                    if (!accountEditDTO.NewPassword.Any(char.IsDigit)) return false;
                    if (!accountEditDTO.NewPassword.Any(char.IsLetterOrDigit)) return false;
                    if (_context.Users.Where(u => u.ID != accountEditDTO.ID).Any(u => u.Email == accountEditDTO.Email)) return false;

                    accountEditDTO.NewPassword = HashPassword($"{accountEditDTO.NewPassword}{user.Salt}");
                }
                _mapper.Map(accountEditDTO, user); // update not null (Automapper)
                _context.SaveChanges();
                return true;
            }
            else return false;           
        }

        public bool CreateReservation(ReservationDTO reservationDTO, int userID)
        {
            Reservation reservation = new Reservation()
            {
                UserID = userID,
                AccommodationID = reservationDTO.AccommodationID,
                RoomID = reservationDTO.RoomID,
                CheckInDate = reservationDTO.CheckIn.Date,
                CheckOutDate = reservationDTO.CheckOut.Date,
                NumberOfGuests = reservationDTO.NumberOfGuests
            };

            var available = !_context
                            .Reservations
                            .Where(x => x.AccommodationID == reservation.AccommodationID && x.RoomID == reservation.RoomID)
                            .Where(x => (x.CheckInDate <= reservation.CheckOutDate) && (x.CheckOutDate >= reservation.CheckInDate))
                            .Where(x => x.IsCancelled == false)
                            .Any();

            if (available)
            {
                _context.Reservations.Add(reservation);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        
        public Guid AddRecordToPasswordResetTable(string email)
        { 
            var thisUser = _context.Users.First(x => x.Email == email);
            var rand = new Random();
            var salt = rand.Next();
            var hashedEmail = HashPassword($"{thisUser.Email}{salt}");
            var thisResetUserPassword = new ResetUserPassword()
            {
                PasswordResetId = Guid.NewGuid(),
                PasswordResetLink = Guid.NewGuid(),
                Email = thisUser.Email,
                UserId = thisUser.ID,
                Salt = salt,
                Secret = hashedEmail,
                DateTime = DateTime.UtcNow,
                IpAdress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()
        };
            _context.ResetUserPasswords.Add(thisResetUserPassword);
            _context.SaveChanges();
            return thisResetUserPassword.PasswordResetLink;
        }

        public bool CheckIfPasswordRecordExists(Guid guid)
        {
            return _context.ResetUserPasswords.Any(x => x.PasswordResetLink == guid);
        }

        public string ReturnSecretOfPasswordRecord(Guid passwordResetLink)
        { 
            var thisRecord = _context.ResetUserPasswords.First(x =>x.PasswordResetLink.Equals(passwordResetLink));
            return thisRecord.Secret;
        }

        public bool CheckIfSecretExistsInPasswordRecordDb(string secret)
        {
            return _context.ResetUserPasswords.Any(x => x.Secret.Equals(secret));
        }

        public bool ChangePasswordFromForgotten(string secret, string pass1, string pass2)
        {
            var thisUserEmail = _context.ResetUserPasswords.First(x => x.Secret.Equals(secret)).Email;
            if (UserInputValidation(pass1, pass2))
            {
                if (ChangePassword(thisUserEmail, pass1))
                    return true;
                else return false;
            }
            else return false;  
        }

        public bool CheckIfTimeStampIsStillValidFromGuidURL(Guid guid)
        {
            var passwordResetRecord = _context.ResetUserPasswords.First(x => x.PasswordResetLink.Equals(guid));
            if (passwordResetRecord.DateTime.AddMinutes(15) > DateTime.UtcNow) return true;
            return false;
        }

        public bool ChangePassword(string email, string password)
        {
            var thisUser = GetUserByEmail(email);
            thisUser.Password = HashPassword($"{password}{thisUser.Salt}");
            _context.Users.Update(thisUser);
            _context.SaveChanges();
            return true;
        }

        public ReturnUserReservationList GetUserReservationList(int userId)
        {
            var reservations = _context.Reservations.Include(x => x.Accommodations).Include(y => y.Room).Where(u => u.UserID== userId && u.IsCancelled == false).ToList();
            var mappedReservations = _mapper.Map<List<ReturnUserReservation>>(reservations);
            ReturnUserReservationList list = new ReturnUserReservationList();
            list.Reservations = mappedReservations;
            return list; 
        }
    }
}
