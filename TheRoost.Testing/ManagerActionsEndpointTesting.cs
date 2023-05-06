using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using TheRoost.API;
using TheRoost.API.AppDbContext;
using TheRoost.API.Configuration;
using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.Entities.PropertyTypes;
using TheRoost.API.Services;


namespace TheRoost.Testing
{
    public class ManagerActionsEndpointTesting : TestingBase
    {
        public ManagerActionsEndpointTesting(CustomWebApplicationFactory<Program> customWebApplicationFactory) : base(customWebApplicationFactory)
        {
        }

        [Theory]
        [InlineData("ManagerActions/view-accommodation/", 1)]
        [InlineData("ManagerActions/view-accommodation/", 2)]
        public async Task ViewAccommodationReturnsStatusCodeOK(string url, int id)
        {
            //Arrange
            var client = _applicationFactory.CreateClient();

            //Act
            var response = await client.GetAsync(url + id);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("ManagerActions/view-accommodation/", 0)]
        public async Task ViewAccommodationReturnsStatusCodeBadRequest(string url, int id)
        {
            //Arrange
            var client = _applicationFactory.CreateClient();

            //Act
            var response = await client.GetAsync(url + id);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RegisterNewPropertyReturnsStatusCodeOK()
        {
            //Arrange
            var client = _applicationFactory.CreateClient();

            //Act
            Uri uri = new Uri("http://farm4.static.flickr.com/2232/2232/someimage.jpg");
            var model = new AccommodationCreationDTO()
            {
                TypeOfProperty = "hostel",
                Phone = "string",
                Name = "string",
                Address = "string",
                City = "string",
                Country = "string",
                Region = "string",
                Description = "string",
                Logo = uri,
                TimeZoneName = "UTC"
            };
            var json = JsonContent.Create(model);
            var response = await client.PostAsync("/ManagerActions/add-new-accommodation", json);

            Assert.NotNull(response);
        }

        [Theory]
        [InlineData("/ManagerActions/add-new-accommodation")]
        public async Task RegisterNewPropertyReturnsStatusCodeBadRequest(string url)
        {
            //Arrange
            var client = _applicationFactory.CreateClient();

            //Act
            Uri uri = new Uri("http://farm4.static.flickr.com/2232/2232/someimage.jpg");
            var model = new AccommodationCreationDTO()
            {
                TypeOfProperty = "asdasdasd",
                Phone = "string",
                Name = "string",
                Address = "string",
                City = "string",
                Country = "string",
                Region = "string",
                Description = "string",
                Logo = uri,
                TimeZoneName = "UTC"
            };
            var json = JsonContent.Create(model);
            var response = await client.PostAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("ManagerActions/edit-accommodation")]
        public async Task UpdateAccommodationReturnsStatusCodeOK(string url)
        {
            //Arrange
            //Get manager
            var user = _context
                .Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.ID == 3);

            //Get random accommodation for the manager, prepare data entry
            var ids = _context.Accommodations.Where(a => a.AccommodationManagerID == user.ID).Select(a => a.ID).ToList();
            var random = new Random();
            var model = new AccommodationUpdateDTO()
            {
                ID = ids[random.Next(ids.Count)],
                City = "123StatusCodeOKTest123"
            };

            //Add authentification and create json
            var token = GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var json = JsonContent.Create(model);

            //Act
            var response = await _client.PostAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/ManagerActions/edit-accommodation")]
        public async Task UpdateAccommodationReturnsStatusCodeBadRequest(string url)
        {
            //Arrange
            //Get manager
            var user = _context
                .Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.ID == 3);
            var token = GenerateToken(user);

            //Prepare data entry
            var model = new AccommodationUpdateDTO()
            {
                ID = _context.Accommodations.Max(a => a.ID) + 1, //get ID which will not exist in DB
                City = "123StatusCodeOKTest123"
            };
            var json = JsonContent.Create(model);

            //Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("/ManagerActions/managed-accommodations")]
        public async Task ViewMyAccommodationsValidInputReturnsStatusCodeOK(string url)
        {
            //Arrange
            //Get manager
            var user = _context
                .Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.ID == 3); //Manager
            //Authentificate
            var token = GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [InlineData("/ManagerActions/view-reservations")] 
        public async Task ViewReservationsManagerTestOk(string url)
        {
            //Arrange
            var user = _context
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(u => u.ID == 2);

            var token = GenerateToken(user);

            //Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync(url);
            
            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/ManagerActions/managed-accommodations")]
        public async Task ViewMyAccommodationsUserUnauthorizedReturnsStatusCodeUnauthorized(string url)
        {
            //Arrange
            //Get manager
            var user = _context
                .Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.ID == 4); //User
            //Authentificate
            var token = GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [InlineData("/ManagerActions/view-reservations")]
        public async Task ViewReservationsManagerTestNotFound(string url)
        {
            //Arrange
            var user = _context
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(u => u.ID == 3);

            var token = GenerateToken(user);
            //Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("/ManagerActions/view-hotel-reservations/1")]
        public async Task ViewReservationsHotelTestOk(string url)
        {
            //Arrange
            var user = _context
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(u => u.ID == 2);

            var token = GenerateToken(user);
            //Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
