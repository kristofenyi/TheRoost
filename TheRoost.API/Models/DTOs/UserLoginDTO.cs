namespace TheRoost.API.Models.DTOs
{
    public class UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public UserLoginDTO()
        {
        }
        public UserLoginDTO(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
