using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using TheRoost.API;
using TheRoost.API.Models.ErrorHandling;

namespace TheRoost.Testing
{
    public class ExceptionHandlingMiddlewareTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _applicationFactory;

        public ExceptionHandlingMiddlewareTest(WebApplicationFactory<Program> applicationFactory)
        {
            _applicationFactory = applicationFactory;
        }

        [Fact]
        public async Task ReturnsOkStatusCode()
        {
            //Arrange
            var client = _applicationFactory.CreateClient();

            //Act
            var response = await client.GetAsync("/swagger");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task TestEmptyViewFor500StatusCode()
        {
            //Arrange
            var client = _applicationFactory.CreateClient();

            //Act
            var response = await client.GetAsync("/");

            //Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task TestExceptionForReturnedJson()
        {
            //Arrange
            var client = _applicationFactory.CreateClient();
            var testError = new ErrorDetailForClient();
            testError.Message = "Internal Server Error, please get in touch with Admin and provide ErrorID";

            //Act
            var response = await client.GetAsync("/");
            var deserializedErrorResponse = await response.Content.ReadFromJsonAsync<ErrorDetailForClient>();

            //Assert
            Assert.Equal(testError.Message, deserializedErrorResponse.Message);
        }
    }
}
