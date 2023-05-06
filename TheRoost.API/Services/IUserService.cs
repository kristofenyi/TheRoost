using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.Entities;

namespace TheRoost.Services
{
    public interface IUserService
    {
        string HashPassword(string password);
        User Login(UserLoginDTO userLogin);
        void RegisterNewUser(UserRegistrationDTO user);
        User GetUserByEmail(string email);
        bool UserInputValidation(string password, string passwordConfirmation);
        bool EmailInputValidation(string email);
        bool CheckIfUserExists(string email);
        Reservation GetReservatoionByID(Guid guid);
        bool CancelReservation(Guid guid);
        string GetUserEmailClaim();
        ReturnUserReservationList GetUserReservationList(int userId);
        bool EditUserAccount(AccountEditDTO accountEditDTO);
        bool CreateReservation(ReservationDTO reservationDTO, int UserID);
        User GetUserByIDClaim();
        int GetUserIDClaim();
        Guid AddRecordToPasswordResetTable(string email);
        bool CheckIfPasswordRecordExists(Guid guid);
        string ReturnSecretOfPasswordRecord(Guid passwordResetLink);
        bool CheckIfSecretExistsInPasswordRecordDb(string secret);
        bool ChangePasswordFromForgotten(string secret, string pass1, string pass2);
        bool ChangePassword(string email, string password);
        bool CheckIfTimeStampIsStillValidFromGuidURL(Guid guid);
    }
}
