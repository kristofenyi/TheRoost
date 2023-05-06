using Microsoft.AspNetCore.Mvc.Testing;
using TheRoost.API;
using TheRoost.API.Models.DTOs;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TheRoost.API.Models.Entities;
using TheRoost.API.AppDbContext;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Net;
using TheRoost.API.Models.ErrorHandling;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheRoost.API.Controllers;

namespace TheRoost.Testing
{
    public class AuthServicesTests : TestingBase
    {
        public AuthServicesTests(CustomWebApplicationFactory<Program> customWebApplicationFactory) : base(customWebApplicationFactory)
        {
        }

        [Theory]
        [InlineData("test@email.com", "Test11")]
        public async Task GenerateTokenTest(string email, string password)
        {
            // Arrange
            var user = new User(email, password);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Jwt_Key"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            // Act
            var token = new JwtSecurityToken("Jwt_Issuer",
              "Jwt_Audience",
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);
            // Assert
            Assert.NotNull(token);
        }

        [Fact]
        public async Task AuthenticateTest()
        {
            // Arrange
            var userLogin = new UserLoginDTO("jane.doe@test.com", "Password1");
            var tryToUsePasswrod = String.Empty;
            User result = null;

            var thisUser = _context
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(u => u.Email == userLogin.Email);

            // Act
            if (thisUser != null)
            {
                tryToUsePasswrod = HashPassword($"{userLogin.Password}{thisUser.Salt}");
                if (tryToUsePasswrod == thisUser.Password)
                {
                    result = thisUser;
                }
                else result = null;
            }
            else result = null;
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TestEndpointNoToken()
        {
            //Arrange
            //Act
            var response = _client.GetAsync("https://localhost:7056/user/test").Result;

            //Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        public string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var asBytes = Encoding.Default.GetBytes(password);
            var hashed = sha.ComputeHash(asBytes);
            return Convert.ToBase64String(hashed);
        }
    }
}
