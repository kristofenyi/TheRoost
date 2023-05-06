using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;
using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.Entities.PropertyTypes;
using TheRoost.API.Models.ErrorHandling;
using TheRoost.API.Services;
using TheRoost.Services;

namespace TheRoost.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManagerActionsController : Controller
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IUserService _userService;
        private readonly IManagerActionService _managerActionService;

        public ManagerActionsController(IAccommodationService accommodationService, IUserService userService, IManagerActionService managerActionService)
        {
            _accommodationService = accommodationService;
            _userService = userService;
            _managerActionService = managerActionService;   
        }

        /// <summary>
        /// Add new property. TypeOfProperty can be: Hotel, Hostel, Apartment or Guesthouse. Case-insensitive. 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     List of all timezones is at:
        ///     https://learn.microsoft.com/en-us/previous-versions/windows/embedded/ms912391(v=winembedded.11)
        ///     POST /add-new-accommodation
        ///     
        ///     {
        ///     "typeOfProperty": "hotel",
        ///     "phone": "string",
        ///     "name": "string",
        ///     "address": "string",
        ///     "country": "string",
        ///     "city": "string",
        ///     "region": "string",
        ///     "description": "Hotel decription",
        ///     "logo": "https://something.com/image.png",
        ///     "timeZoneName": "UTC"
        ///     }
        ///    
        /// </remarks>
        [HttpPost("add-new-accommodation")]
        public IActionResult AddAccommodation(AccommodationCreationDTO accommodation)
        {
            if (_accommodationService.SortAndAddToDB(accommodation))
                return Ok();
            else return BadRequest();
        }

        [HttpGet("view-accommodation/{id}")]
        public IActionResult ViewAccommodation(int id)
        {
            //TODO: If the user is not a hotel manager or not the manager of this property they should be redirected to the home page.
            // Add Authorization, after JWT is implemented to check, if the Manager can view this property
            if (_accommodationService.AccommodationIDExists(id))
                return Ok(_accommodationService.GetAccommodationDTOByID(id));
            else return BadRequest();
        }

        /// <summary>
        /// Adds list of rooms to accomodation. Checks Role, manager vs. accomodation, room numbers
        /// </summary>
        [HttpPost("add-room")]
        [Authorize(Roles = "Hotel Manager")]
        public IActionResult AddRoom([FromBody] RoomCreationDTO roomDTO)
        {
            var managerAuthorization = _accommodationService.ManagerAuthorization(roomDTO);
            var roomValidation = _accommodationService.RoomValidation(roomDTO);

            if (!managerAuthorization)
                return Unauthorized();
            if (!roomValidation)
                return BadRequest();

            foreach (var r in roomDTO.Rooms)
            {
                _accommodationService.CreateRoom(r);
            }
            return Ok();
        }

        /// <summary>
        /// Edits existing property. Property matching is done by ID.
        /// </summary>
        [HttpPost("edit-accommodation")]
        [Authorize(Roles = "Hotel Manager")]
        public IActionResult UpdateAccommodation([FromBody] AccommodationUpdateDTO accommodation)
        {
            bool success = _accommodationService.UpdateAccommodation(accommodation);
            if (!success)
                return BadRequest();
            else
                return Ok("Accommodation with ID: " + accommodation.ID + " has been updated.");
        }

        /// <summary>
        /// Lists accommodations managed by logged in user. No matches equals empty result.        
        /// </summary>
        [Authorize(Roles = "Hotel Manager")]
        [HttpGet("managed-accommodations/")]
        public IActionResult ViewMyAccommodations()
        {
            return Ok(_accommodationService.GetAccommodationsForUserByID(_userService.GetUserIDClaim()));
        }

        [HttpGet("view-reservations")]
        [Authorize(Roles = "Hotel Manager")]
        public IActionResult ViewReservations()
        {
            User manager = _userService.GetUserByIDClaim();
            var returnReservationsDTO = _managerActionService.GetReservationsForManager(manager.ID);

            if (returnReservationsDTO.Count() > 0)
                return Ok(returnReservationsDTO);
            else return NotFound();
        }

        [HttpGet("view-hotel-reservations/{id}")]
        [Authorize(Roles = "Hotel Manager")]
        public IActionResult ViewReservationsOnHotel([FromRoute] int id)
        {
            var validation = _managerActionService.ValidateManagerAccommodation(_userService.GetUserByIDClaim().ID, id);

            if (validation)
            {
                var returnReservationsDTO = _managerActionService.GetReservationsForHotel(id);

                if (returnReservationsDTO.Count() > 0)
                    return Ok(returnReservationsDTO);
                else return NotFound();
            } else return Unauthorized();
        }
    }
}
