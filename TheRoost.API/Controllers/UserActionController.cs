using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.ErrorHandling;
using TheRoost.API.Services;
using TheRoost.Services;

namespace TheRoost.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserActionController : Controller
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        public UserActionController(IAccommodationService accommodationService, IUserService userService, IEmailService emailService)
        {
            _accommodationService = accommodationService;
            _userService = userService;
            _emailService= emailService;
        }

        [HttpPost("create-reservation")]
        [Authorize(Roles = "User")]
        public IActionResult CreateReservation([FromBody] ReservationDTO reservationDTO)
        {
            User user = _userService.GetUserByIDClaim();
            var result = _userService.CreateReservation(reservationDTO, user.ID);

            if (result)
            {
                _emailService.SendEmailReservationNotification(reservationDTO);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("cancel-reservation/{guid}")]
        public IActionResult CancelReservation([FromRoute] Guid guid)
        {
            var result = _userService.CancelReservation(guid);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("view-reservations")]
        [Authorize(Roles = "User")]
        public IActionResult GetReservation()
        {
                var reservationsList = _userService.GetUserReservationList(_userService.GetUserByIDClaim().ID);
                return Ok(reservationsList);
        }

        [HttpGet("reset-password")]
        public IActionResult ResetPassword()
        {
            return BadRequest();
        }

        [HttpPost("add-review")]
        [Authorize(Roles = "User")]
        public IActionResult AddAccommodationReview([FromBody] AccommodationReviewDTO reviewDTO)
        {
            //Read user ID from JWT claim using UserService
            int userID = _userService.GetUserByEmail(_userService.GetUserEmailClaim()).ID;

            //Check requests validity and decide if review can be added
            if (_accommodationService.IsReviewRequestValid(reviewDTO, userID))
            {
                _accommodationService.AddNewReview(reviewDTO, userID);
                return Ok();
            } else
            {
                return BadRequest();
            }
        }
    }
}
