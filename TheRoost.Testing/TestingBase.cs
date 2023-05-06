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
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TheRoost.Testing
{

    public class TestingBase : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        protected readonly CustomWebApplicationFactory<Program> _applicationFactory;
        protected IConfiguration _config;
        protected readonly MainDbContext _context;
        protected readonly HttpClient _client;

        public TestingBase(CustomWebApplicationFactory<Program> customWebApplicationFactory)
        {
            _applicationFactory = customWebApplicationFactory;
            _context = TestingContextProvider.CreateContextFromFactory(_applicationFactory);
            _config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .Build();
            _client = _applicationFactory.CreateClient();
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

            var token = new JwtSecurityToken(_config["Jwt_Issuer"], _config["Jwt_Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    
}
