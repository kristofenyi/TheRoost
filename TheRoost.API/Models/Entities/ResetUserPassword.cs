using System.ComponentModel.DataAnnotations;

namespace TheRoost.API.Models.Entities
{
    public class ResetUserPassword
    {
        [Key]
        public Guid PasswordResetId { get; set; }
        public Guid PasswordResetLink { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Secret { get; set; }
        public int Salt { get; set; }
        public DateTime DateTime { get; set; }
        public string IpAdress { get; set; }
    }
}
