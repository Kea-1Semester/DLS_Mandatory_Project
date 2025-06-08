using AuthClassLibrary;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogInService _logInService;
    private readonly IConfiguration _config;
    public AuthController(ILogInService logInService, IConfiguration config)
    {
        _logInService = logInService;
        _config = config;
    }

    /// <summary>
    /// Retrieves authentication information.
    /// </summary>
    /// <param name="user">User object containing email and password.</param>
    /// <returns>Response with authentication information.</returns>
    [HttpPost("login")]
    //[ValidateAntiForgeryToken]
    public async Task<ActionResult<AuthResult>> Login([FromBody] LogIn user)
    {
        if (user == null)
        {
            return BadRequest("Invalid user data.");
        }
        var authResult = await _logInService.Login(user.Email, user.Password);
        if (authResult == null)
        {
            return Unauthorized("Invalid email or password.");
        }
        return Ok(authResult);
    }



    [HttpGet("getstringFromAuthService")]
    public string GetString()
    {   
        return "This is a string response from auth";
    }

}
