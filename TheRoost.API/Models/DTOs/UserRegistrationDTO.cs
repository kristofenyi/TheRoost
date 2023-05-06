namespace TheRoost.API.Models.DTOs
{
    public class UserRegistrationDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}