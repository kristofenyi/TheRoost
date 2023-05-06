using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net;
using TheRoost.API;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TheRoost.Testing
{
    public class ReservationTesting : TestingBase
    {
        public ReservationTesting(CustomWebApplicationFactory<Program> customWebApplicationFactory) : base(customWebApplicationFactory)
        {
        }

        [Theory]
        [InlineData("UserAction/create-reservation")]
        public async Task CreateReservationTestBadRequest(string url) // reservation conflict on booking interval
        {
            //Arrange
            var user = _context
                .Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.ID == 4);

            ReservationDTO reservationDTO = new ReservationDTO
            {
                AccommodationID = 1,
                RoomID = 5,
                CheckIn = new DateTime(2022, 12, 25),
                CheckOut = new DateTime(2022, 12, 30),
                NumberOfGuests = 2
            };

            var json = JsonContent.Create(reservationDTO);

            //Act
            var token = GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("UserAction/create-reservation")]
        public async Task CreateReservationTestOk(string url)
        {
            //Arrange
            var user = _context
                .Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.ID == 4);

            ReservationDTO reservationDTO = new ReservationDTO
            {
                AccommodationID = 2,
                RoomID = 5,
                CheckIn = new DateTime(2023, 12, 25),
                CheckOut = new DateTime(2023, 12, 30),
                NumberOfGuests = 2
            };

            var json = JsonContent.Create(reservationDTO);

            //Act
            var token = GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
