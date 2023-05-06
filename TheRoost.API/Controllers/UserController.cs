using Microsoft.AspNetCore.Mvc;
using TheRoost.Services;
using TheRoost.API.Models.DTOs;
using TheRoost.API.Services;
using Microsoft.AspNetCore.Authorization;
using TheRoost.API.Models.ErrorHandling;
using System.IdentityModel.Tokens.Jwt;
using TheRoost.API.Models.Entities;
using System.Security.Claims;

namespace TheRoost.API.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        
        public UserController(IUserService userService, IAuthService authService, IEmailService emailService)
        {
            _userService = userService;
            _authService = authService;
            _emailService = emailService;
        }

        [HttpPost("registration")]
        public IActionResult RegisterUser([FromBody] UserRegistrationDTO userRegister)
            //DTO for login register and/or login
        {
            var exists = _userService.CheckIfUserExists(userRegister.Email);
            var passwordValidation = _userService.UserInputValidation(userRegister.Password, userRegister.PasswordConfirmation);
            var emailValidation = _userService.EmailInputValidation(userRegister.Email);
            
            if (!exists && passwordValidation && emailValidation)
            {
                _userService.RegisterNewUser(userRegister);
                //TODO: Use ErrorModel
                _emailService.SendEmailRegistrationSuccessful(userRegister.Email);
                return Ok(new RequestResponse { Message = "Registration was succesful" });
            }
            else
            {
                return BadRequest(new RequestResponse { Message = "Registration failed" });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDTO userLogin)
        {
            var user = _authService.AuthenticateUser(userLogin);

            if (user != null)
            {
                var token = _authService.GenerateToken(user);
                return Ok(token);
            }

            return NotFound(new RequestResponse { Message = "User not found" });
        }

        [AllowAnonymous]
        [HttpPut("edit-account")]
        public IActionResult EditAccount([FromBody] AccountEditDTO accountEdit)
        {
            User user = _userService.GetUserByIDClaim();

            if (user.ID == accountEdit.ID)
            {
                var result = _userService.EditUserAccount(accountEdit);

                if (result)
                {
                    return Ok(new RequestResponse { Message = "Account updated" });
                }
                else
                {
                    return BadRequest();
                }
            } return Unauthorized();
        }
    }
}
