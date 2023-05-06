using Microsoft.AspNetCore.Mvc.Testing;
using TheRoost.API;
using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.Entities;
using TheRoost.API.AppDbContext;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TheRoost.API.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net.Http.Headers;
using Azure.Core;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace TheRoost.Testing
{
    public class EditUserAccountTesting : TestingBase
    {
        public EditUserAccountTesting(CustomWebApplicationFactory<Program> customWebApplicationFactory) : base(customWebApplicationFactory)
        {
        }

        [Theory]
        [InlineData("/user/edit-account")] 
        public async Task EditAccountTestOk(string url)
        {
            //Arrange
            AccountEditDTO accountEditDTO = new AccountEditDTO()
            {
                ID = 5,
                Name = "Test",
                Email = "john@test.com",
                OldPassword = "Password1",
                NewPassword = "Testik11",
                PasswordConfirmation = "Testik11"
            };

            var user = _context
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.ID == 5);
            var token = GenerateToken(user);

            var json = JsonContent.Create(accountEditDTO);
            //Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/user/edit-account")]
        public async Task EditAccountTestBadRequest(string url) //password confirmation wrong
        {
            //Arrange
            AccountEditDTO accountEditDTO = new AccountEditDTO()
            {
                ID = 3,
                Name = "Test",
                Email = "email@email.com",
                OldPassword = "Testik1",
                NewPassword = "Testik",
                PasswordConfirmation = "Testik1"
            };

            var user = _context
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.ID == 3);
            var token = GenerateToken(user);

            var json = JsonContent.Create(accountEditDTO);
            //Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("/user/edit-account")]
        public async Task EditAccountTestUnathorized(string url) //unauthorized by ID token owner
        {
            //Arrange
            AccountEditDTO accountEditDTO = new AccountEditDTO()
            {
                ID = 2,
                Name = "Test",
                Email = "email@email.com",
                OldPassword = "Testik1",
                NewPassword = "Testik1",
                PasswordConfirmation = "Testik1"
            };

            var user = _context
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.ID == 3);
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
