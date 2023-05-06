using Microsoft.AspNetCore.Mvc;
using TheRoost.API.Models.ErrorHandling;
using TheRoost.API.Models.Json;
using TheRoost.API.Services;
using TheRoost.Services;

namespace TheRoost.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GuestActionController : Controller
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly RoomService _roomService;

        public GuestActionController(IAccommodationService accommodationService,
            IUserService userService,
            IEmailService emailService,
            RoomService roomService)
        {
            _accommodationService = accommodationService;
            _userService = userService;
            _emailService = emailService;
            _roomService = roomService;
        }
        
        [HttpGet("view-accommodation/{id}")]
        public IActionResult ViewAccommodation(int id)
        {
            if (_accommodationService.AccommodationIDExists(id))
                return Ok(_accommodationService.GetAccommodationDTOByID(id));
            else return BadRequest();
        }

        /// <summary>
        /// Searches for hotels based on a name of a city and (optionally) maximum capacity. Guest count must be above zero. 
        /// No matches equals empty result. Returns list sorted alphabetically by name.
        /// </summary>
        [HttpGet("hotels/")]
        public IActionResult SearchHotels([FromQuery] string city, [FromQuery] int? guestCount)
        {
            //Returns List<Hotel> by capacity requirements
            city = city.Trim();
            if (_accommodationService.ValidateUserInputForSearching(city, guestCount))
                return Ok(_accommodationService.GetAccommodationsByCityAndGuestCount(city, guestCount));
            else
                return BadRequest();
        }

        /// <summary>
        /// Returns available rooms. Do not input time
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     Date Format:
        ///     2022-06-15
        ///     Do not input time
        /// </remarks>
        [HttpGet("check-available-rooms")]
        public IActionResult AllAvailableRooms(string from, string to)
        {
            if (_roomService.IsCorrectDateFormat(from, to))
            {
                var checkAvailabilityJson = _roomService.TransfromDatesToJson(from, to);
            
            if (!_roomService.CheckIfDatesAreValid(checkAvailabilityJson.From, checkAvailabilityJson.To))
                return BadRequest(new RequestResponse { Message = "Invalid Date format" });
                var myObject = _roomService.ReturnAvailableRooms(checkAvailabilityJson.From, checkAvailabilityJson.To);
            if (myObject.Count < 1)
            {
                return Ok(new RequestResponse { Message = "No rooms available in selected date range" });
            }
            else return Ok(myObject);
            }
            else return BadRequest(new RequestResponse { Message = "Invalid Date format"});
        }

        [HttpPost("forgotten-password")]
        public IActionResult ForgottenPassword(string email)
        {
            //Check if email exists in DB
            if (_userService.CheckIfUserExists(email))
            {
                //Add to DB userID, GUID + timestamp
                var guid = _userService.AddRecordToPasswordResetTable(email);
                //Send email to customer with reset link, where GUID is the in the code
                _emailService.SendEmailForgottenPassword(email,guid);
                //Return Ok if email exists + instructions
                return Ok();
            }
            //Return Bad request, email does not exist
            else return BadRequest(new ErrorDetailForClient { Message = "Email is not valid" });
        }

        [HttpGet("get-reset-secret/{guid}")]
        public IActionResult GetResetSecret(Guid guid)
        {
            //check if guid is in the password reset DB
            if (_userService.CheckIfPasswordRecordExists(guid) && _userService.CheckIfTimeStampIsStillValidFromGuidURL(guid))
            {
                //if yes, give him a secret to use in the password reset / hasehed something
                return Ok(_userService.ReturnSecretOfPasswordRecord(guid));
            }
            else return BadRequest();
        }

        [HttpPut("confirm-reset-password")]
        public IActionResult ResetPassword(string secret, string newpass1, string newpass2)
        {
            //check if secret is valid
            if (_userService.CheckIfSecretExistsInPasswordRecordDb(secret))
            {
                //if yes let him make a PUT request
                //From secret load user and let him change password
                if (_userService.ChangePasswordFromForgotten(secret, newpass1, newpass2))
                {
                    return Ok();
                }
                else return BadRequest();
            }
            return BadRequest(new ErrorDetailForClient { Message = "Invalid secret" });
        }
    }
}
