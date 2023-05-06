using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using TheRoost.API;
using TheRoost.API.AppDbContext;
using TheRoost.API.Configuration;
using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.Entities.PropertyTypes;
using TheRoost.API.Services;

namespace TheRoost.Testing
{
    public class CancelReservationTesting : TestingBase
    {
        public CancelReservationTesting(CustomWebApplicationFactory<Program> customWebApplicationFactory) : base(customWebApplicationFactory)
        {
        }

        [Theory]
        [InlineData("UserAction/cancel-reservation/152821e6-8a86-418b-d281-08daf46d9e21", "152821e6-8a86-418b-d281-08daf46d9e21")]
        public async Task CancelReservationStatusCodeOK(string url, Guid guid)
        {
            //Arrange
            var user = _context
                .Users
                .Include(x => x.Role)
                .FirstOrDefault();

            var token = GenerateToken(user);

            //Act
            var json = JsonContent.Create(guid);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync(url, json);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
