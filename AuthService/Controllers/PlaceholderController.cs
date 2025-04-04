using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class PlaceholderController : ControllerBase
{

    [HttpGet("getstring")]
    public string GetString()
    {
        return "This is a string response";
    }


}
