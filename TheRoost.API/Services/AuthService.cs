using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using TheRoost.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using TheRoost.API.AppDbContext;
using TheRoost.API.Models.Entities;
using TheRoost.Services;

namespace TheRoost.API.Services
{
    public class AuthService : IAuthService
    {
        private IConfiguration _config;
        private IUserService _userService;
        private readonly MainDbContext _context;

        public AuthService()
        {
        }

        public AuthService(MainDbContext context, IUserService userService, IConfiguration config)
        {
            _context = context;
            _userService = userService;
            _config = config;
        }
        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt_Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Title)
            };

            var token = new JwtSecurityToken(_config["Jwt_Issuer"],
              _config["Jwt_Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public User AuthenticateUser(UserLoginDTO userLogin)
        {
            var thisUser = _context
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(u => u.Email == userLogin.Email);

            if (thisUser != null) 
            {
                var tryToUsePassword = _userService.HashPassword($"{userLogin.Password}{thisUser.Salt}");
                if (tryToUsePassword == thisUser.Password)
                {
                    return thisUser;
                } 
                else return null;                
            } 
            else return null;
        }
    }
}
