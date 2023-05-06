using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.Entities;

namespace TheRoost.API.Services
{
    public interface IAuthService
    {
        string GenerateToken(User user);
        User AuthenticateUser(UserLoginDTO userLogin);
    }
}
