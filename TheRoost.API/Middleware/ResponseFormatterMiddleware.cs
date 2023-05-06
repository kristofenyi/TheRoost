using Azure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace TheRoost.API.Middleware
{
    public class ResponseFormatterMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseFormatterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);

            var test = context.User.Identity.IsAuthenticated;

            if (context.Request.Path == "/user/edit-account") 
            {
                switch (context.Response.StatusCode)
                {
                    case StatusCodes.Status401Unauthorized:
                        await context.Response.WriteAsync("User ID conflict.");
                        break;
                    case StatusCodes.Status400BadRequest:
                        await context.Response.WriteAsync("Input values are incorrect, make sure the passwords are correct.");
                        break;             
                }                
            }
            if (context.Request.Path == "/UserAction/create-reservation")
            {
                switch (context.Response.StatusCode)
                {
                    case StatusCodes.Status200OK:
                        await context.Response.WriteAsync("Reservation canceled.");
                        break;
                    case StatusCodes.Status400BadRequest:
                        await context.Response.WriteAsync("Sorry the reservation cannot be canceled bt now.");
                        break;
                }
            }
            if (context.Request.Path == "/UserAction/cancel-reservation")
            {
                switch (context.Response.StatusCode)
                {
                    case StatusCodes.Status200OK:
                        await context.Response.WriteAsync("Reservation created successfuly.");
                        break;
                    case StatusCodes.Status400BadRequest:
                        await context.Response.WriteAsync("Sorry the room is already booked in this time.");
                        break;
                }
            }
            else if (context.Request.Path == "/UserAction/add-review")
            {
                if (context.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    await context.Response.WriteAsync(
                        "Sorry, you can review only accommodations in which you have already concluded your stay. " +
                        "Only one review per accommodation is possible and rating must be between 1 and 5 stars.");
                }
            }
            else if (context.Request.Path.ToString().StartsWith("/GuestAction/hotels"))
            {
                if (context.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    await context.Response.WriteAsync("Search query did not conform to requirements");
                }
            }
            else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                await context.Response.WriteAsync("Authentification token is required");
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Authentification token is invalid");
            }
            else if (context.Request.Path == "/ManagerActions/add-room")
            {
                switch (context.Response.StatusCode)
                {
                    case StatusCodes.Status401Unauthorized:
                        await context.Response.WriteAsync("Accomodation ID conflict");
                        break;
                    case StatusCodes.Status400BadRequest:
                        await context.Response.WriteAsync("Room number conflict.");
                        break;
                }
            }
            else if (context.Request.Path == "/ManagerActions/view-reservations" && context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                await context.Response.WriteAsync("There are no reservations for your hotels.");
            }
            else if (context.Request.Path.StartsWithSegments("/ManagerActions/view-hotel-reservations"))
            {
                switch (context.Response.StatusCode)
                {
                    case StatusCodes.Status401Unauthorized:
                        await context.Response.WriteAsync("Unauthorized access to the accommodation");
                        break;
                    case StatusCodes.Status404NotFound:
                        await context.Response.WriteAsync("No reservations were found");
                        break;
                }
            }
            else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                await context.Response.WriteAsync("Authentification token is required");
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Authentification token is invalid");
            }
        }
    }

    public class ResponseModel
    {
        public ResponseModel(string message)
        {
            this.Message = message;
        }

        public string Message { get; set; }
    }
}
