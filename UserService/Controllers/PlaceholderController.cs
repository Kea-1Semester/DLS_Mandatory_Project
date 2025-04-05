using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class PlaceholderController : ControllerBase
{
    [HttpGet("getstringFromUserService")]
    public string GetString()
    {
        return "This is a string response from user";
    }
}
