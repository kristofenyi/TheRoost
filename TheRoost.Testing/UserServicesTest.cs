using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using TheRoost.API;
using TheRoost.API.Models.DTOs;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using TheRoost.API.Models.Entities;
using TheRoost.API.AppDbContext;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TheRoost.Testing
{
    public class UserServicesTest : TestingBase
    {
        public UserServicesTest(CustomWebApplicationFactory<Program> customWebApplicationFactory) : base(customWebApplicationFactory)
        {
        }

        [Fact]
        public async Task TestHashPassword()
        {
            // Arrange         
            var passwordTest = "passwordTest";

            // Act
            using var sha = SHA256.Create();
            var asBytes = Encoding.Default.GetBytes(passwordTest);
            var hashed = sha.ComputeHash(asBytes);
            Convert.ToBase64String(hashed);

            // Assert
            Assert.NotNull(Convert.ToBase64String(hashed));
        }

        [Theory]
        [InlineData("user/registration")]
        public async Task RegisterNewUserTest(string url)
        {
            // Arrange
            var user = new UserRegistrationDTO()
            {
                Email = "test@testa.com",
                Password = "testTest12",
                PasswordConfirmation = "testTest12",
            };
            var json = JsonContent.Create(user);
            // Act
            var response = await _client.PostAsync(url, json);
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("user/login")]
        public async Task UserLoginTest(string url)
        {
            // Arrange
            var user = new UserLoginDTO()
            {
                Email = "eli.doe@test.com",
                Password = "Password1"
            };
            var json = JsonContent.Create(user);
            // Act
            var response = await _client.PostAsync(url, json);
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/user/edit-account")]
        public async Task EditAccountTest(string url)
        {
            //Arrange
            AccountEditDTO accountEditDTO = new AccountEditDTO()
            {
                ID = 1,
                Name = "Test",
                Email = "kimble@test.com",
                OldPassword = "Password1",
                NewPassword = "Testik1",
                PasswordConfirmation = "Testik1"
            };

            var user = _context
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Email == "kimble@test.com");

            var token = GenerateToken(user);

            var json = JsonContent.Create(accountEditDTO);
            //Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
