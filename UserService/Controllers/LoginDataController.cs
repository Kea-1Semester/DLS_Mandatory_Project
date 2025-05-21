using Microsoft.AspNetCore.Mvc;
using UserClassLibrary;
using UserService.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserService.Controllers
{
    /// <summary>
    /// Controller for handling login data retrieval.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class LoginDataController : ControllerBase
    {
        private readonly UserQueries _userQueries;

        /// <summary>
        /// Constructor for LoginDataController.
        /// </summary>
        /// <param name="userQueries">The UserQueries service for database operations.</param>
        public LoginDataController(UserQueries userQueries)
        {
            _userQueries = userQueries;
        }


        /// <summary>
        /// Retrieves login data (email and hashed password) for a user by email.
        /// </summary>
        /// <param name="userLogin">The email address of the user.</param>
        /// <returns>
        /// Returns an HTTP 200 OK response with the user's login data if found.
        /// Returns an HTTP 404 Not Found response if the user does not exist.
        /// </returns>
        [HttpPost("login-data/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLoginData([FromBody] UserLoginInfo userLogin)
        {
            try
            {
                UserInfo userlogin = new(email: userLogin.Email, password: userLogin.Password);
                userlogin.ValidateEmail();
                userlogin.ValidatePassword();

                // Retrieve the user by email
                dynamic userData = await _userQueries.GetUserByEmail(userlogin.Email);
                if (userData == null)
                {
                    System.Diagnostics.Debug.WriteLine("User not found");

                    return NotFound(new { error = "Invalid email or password, please try again" });
                }


                // Check if the password is correct
                if (!BCrypt.Net.BCrypt.Verify(userlogin.Password, userData.HashPassword))
                {
                    return Unauthorized(new { error = "Invalid email or password, please try again" });
                }

                // Return the user's login data
                return Ok(new
                {
                    Guid = userData.Guid,
                    Email = userData.Email,
                    UserRole = userData.UserRoleCsv,
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Error retrieving login data: {ex.Message}" });
            }


        }
    }
}
