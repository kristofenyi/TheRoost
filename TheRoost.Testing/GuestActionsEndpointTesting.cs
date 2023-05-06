using Azure;
using System.Net;
using System.Text.Json;
using TheRoost.API;
using TheRoost.API.AppDbContext;
using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.ErrorHandling;
using TheRoost.API.Models.Json;

namespace TheRoost.Testing
{
    public class GuestActionsEndpointTesting : TestingBase
    {
        public GuestActionsEndpointTesting(CustomWebApplicationFactory<Program> customWebApplicationFactory) : base(customWebApplicationFactory)
        {
        }

        [Theory]
        [InlineData("GuestAction/view-accommodation/", 1)]
        [InlineData("GuestAction/view-accommodation/", 2)]
        public async Task ReturnsOkStatusCode(string url, int id)
        {
            //Arrange
            var client = _applicationFactory.CreateClient();
            //Act
            var response = await client.GetAsync(url + id);
            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("Praha")]
        public async Task SearchByCityValidInputReturnsStatusCodeOK(string city)
        {
            //Arrange and Act
            var response = await _client.GetAsync($"GuestAction/hotels?city={city}");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("U")] // search term too short
        [InlineData("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX")] // search term too large
        public async Task SearchByCityInvalidInputReturnsStatusCodeBadRequest(string city)
        {
            //Arrange and Act
            var response = await _client.GetAsync($"GuestAction/hotels?city={city}");

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("Canggu", 1)]
        [InlineData("ggu", 1)]
        [InlineData("gGu   ", 1)]
        [InlineData("gu", 1)]
        [InlineData(" CaN", 1)]
        [InlineData("DefinitelyNotACity", 0)]
        [InlineData("NY", 3)]
        [InlineData("ny", 3)]
        public async Task SearchByCityValidInputReturnsValidResults(string city, int expectedCount)
        {
            //Arrange and Act
            var response = await _client.GetAsync($"GuestAction/hotels?city={city}");

            //Assert
            var stream = await response.Content.ReadAsStreamAsync();
            var responseDeserialized = JsonSerializer.Deserialize<List<AccommodationSearchDTO>>(stream);
            Assert.Equal(expectedCount, responseDeserialized.Count);
        }

        [Theory]
        [InlineData("NY", 2, 3)]
        [InlineData("NY", 6, 2)]
        [InlineData("NY", 10, 1)]
        [InlineData("NY", 100, 0)]
        [InlineData("NotACityByAnyChance", 5, 0)]
        public async Task SearchByCityAndGuestCountValidInputReturnsValidResults(string city, int guestCount, int expectedCount)
        {
            //Arrange and Act
            var result = await _client.GetAsync($"GuestAction/hotels?city={city}&guestCount={guestCount}");

            //Assert
            var stream = await result.Content.ReadAsStreamAsync();
            var responseDeserialized = JsonSerializer.Deserialize<List<AccommodationSearchDTO>>(stream);
            Assert.Equal(expectedCount, responseDeserialized.Count);
        }

        [Theory]
        [InlineData("NY", 1000)] // too many guests
        [InlineData("NY", 0)] // zero guests
        [InlineData("NY", -100)] // negative guests
        public async Task SearchByCityAndGuestCountInvalidInputReturnsStatusCodeBadRequest(string city, int guestCount)
        {
            //Arrange and Act
            var response = await _client.GetAsync($"GuestAction/hotels?city={city}&guestCount={guestCount}");

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [InlineData("GuestAction/view-accommodation/", 4)]
        public async void ViewAccomodationHasTimeZoneInfo(string url, int id)
        {
            //Arrange
            var client = _applicationFactory.CreateClient();
            //Act
            var response = await client.GetAsync(url + id);
            var returnedModel = response.Content.ReadFromJsonAsync<ReturnAccommodationDTO>();
            //Assert
            Assert.NotNull(returnedModel.Result.TimeZone);
        }

        [Theory]
        [InlineData("GuestAction/check-available-rooms/", "2022 - 06 - 15", "2022 - 06 - 20")]
        public async Task CheckRoomsAreAvailable(string url, string from, string to)
        {
            //Arrange
            var client = _applicationFactory.CreateClient();
            //Act
            var response = await client.GetAsync($"{url}?from={from}&to={to}");
            var returnedModel = await response.Content.ReadFromJsonAsync<List<ReturnRoomDTO>>();
            //Assert
            Assert.True(returnedModel.Count > 0);
        }

    }
}
