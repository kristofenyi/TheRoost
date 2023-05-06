namespace TheRoost.API.Models.DTOs
{
    public class AccountEditDTO
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? PasswordConfirmation { get; set; }
    }
}
