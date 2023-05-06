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
    public class ManagerActionsAddRoomTesting : TestingBase
    {
        public ManagerActionsAddRoomTesting(CustomWebApplicationFactory<Program> customWebApplicationFactory) : base(customWebApplicationFactory)
        {
        }

        [Theory]
        [InlineData("/ManagerActions/add-room")] //Accomodation ID Manager conflict
        public async Task AddRoomTestUnauthorizedByAccManager(string url)
        {
            //Arrange
            var user = _context
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(u => u.Email == "jane.doe@test.com");

            var token = GenerateToken(user);

            Room room1 = new Room()
            {
                RoomNumber = 421222,
                Description = "room1",
                Price = 10,
                MaxCapacity = 2,
                AccommodationID = 3,
                RoomTypeID = 1,
                ManagerID = 2
            };

            Room room2 = new Room()
            {
                RoomNumber = 33201,
                Description = "room2",
                Price = 12,
                MaxCapacity = 1,
                AccommodationID = 2,
                RoomTypeID = 1,
                ManagerID = 2
            };

            List<Room> rooms = new List<Room>() { room1, room2 };
            RoomCreationDTO roomCreationDTO = new RoomCreationDTO(rooms);
            var json = JsonContent.Create(roomCreationDTO);
            //Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("/ManagerActions/add-room")] //room number conflict
        public async Task AddRoomTestUnauthorizedManagerBadRequestByRoom(string url)
        {
            //Arrange
            var user = _context
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(u => u.Email == "jane.doe@test.com");

            var token = GenerateToken(user);

            Room room1 = new Room()
            {
                RoomNumber = 101,
                Description = "room1",
                Price = 10,
                MaxCapacity = 2,
                AccommodationID = 1,
                RoomTypeID = 1,
                ManagerID = 2
            };

            Room room2 = new Room()
            {
                RoomNumber = 102,
                Description = "room2",
                Price = 12,
                MaxCapacity = 1,
                AccommodationID = 1,
                RoomTypeID = 1,
                ManagerID = 2
            };

            List<Room> rooms = new List<Room>() { room1, room2 };
            RoomCreationDTO roomCreationDTO = new RoomCreationDTO(rooms);
            var json = JsonContent.Create(roomCreationDTO);
            //Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
