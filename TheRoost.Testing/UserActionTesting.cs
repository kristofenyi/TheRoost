using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using TheRoost.API;
using TheRoost.API.AppDbContext;
using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.Entities.PropertyTypes;

namespace TheRoost.Testing
{
    public class UserActionTesting : TestingBase
    {
        public UserActionTesting(CustomWebApplicationFactory<Program> customWebApplicationFactory) : base(customWebApplicationFactory)
        {
        }

        [Theory]
        [InlineData("UserAction/add-review")]
        public async Task AddAccommodationReviewValidInputValidOutput(string url)
        { //User (id = 4) did stay in the accommodation (id = 4) an has not reviewed it yet => they can add a review
            //Arrange
            var user = _context
                .Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.ID == 4);

            Review review = new Review
            {
                AccommodationID = 4,
                DateCreated = DateTime.UtcNow,
                Rating = 4,
                Text = "A review text",
                UserID = user.ID
            };
            AccommodationReviewDTO accommodationCreationDTO = new AccommodationReviewDTO(review);
            var json = JsonContent.Create(accommodationCreationDTO);
            var token = GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("UserAction/add-review")]
        public async Task AddAccommodationReviewUserNotAuthorizedReturnsStatusCodeBadUnauthorized(string url)
        {
            //Arrange
            var user = _context
                .Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.ID == 4);

            Review review = new Review
            {
                AccommodationID = 4,
                DateCreated = DateTime.UtcNow,
                Rating = 4,
                Text = "A review text",
                UserID = user.ID
            };
            AccommodationReviewDTO accommodationCreationDTO = new AccommodationReviewDTO(review);
            var json = JsonContent.Create(accommodationCreationDTO);
            var token = "DefinitelyNotAValidToken";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(GetReviewDataForbiddenUserAction))]
        public async Task AddAccommodationReviewForbiddenInputReturnsStatusCodeBadRequest(int userId, int accommodationID, int rating)
        {
            //Arrange
            var user = _context
                .Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.ID == userId);

            Review review = new Review
            {
                AccommodationID = accommodationID,
                DateCreated = DateTime.UtcNow,
                Rating = rating,
                Text = "A review text",
                UserID = user.ID
            };
            AccommodationReviewDTO accommodationCreationDTO = new AccommodationReviewDTO(review);
            var json = JsonContent.Create(accommodationCreationDTO);
            var token = GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync("UserAction/add-review", json);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("UserAction/view-reservations")]
        public async Task ViewReservationsOk(string url)
        {
            //Arrange
            var user = _context
                .Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.ID == 4);

            var token = GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public static IEnumerable<object[]> GetReviewDataForbiddenUserAction()
        {
            var allData = new List<object[]>
            {
                new object[] { 5, 2, 5 }, //User has not stayed in accommodation
                new object[] { 5, 1, 5 }, //User has already reviewed the accommodation
                new object[] { 4, 2, 5 }, //User is currently staying in accommodation
                new object[] { 4, 3, 5 }, //User had a reservation, but it has been cancelled
                new object[] { 4, 4, 7 }, //User attempts to rate more than 5 stars 
                new object[] { 4, 4, -5 }, //User attempts to rate less than 1 star 
            };
            return allData;
        }
    }
}
