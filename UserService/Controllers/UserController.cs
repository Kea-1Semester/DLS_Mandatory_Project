using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UserService.Models.DTO;
using UserService.Service;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UserQueries _userQueries;
    private readonly UserCommands _userCommands;

    public UserController(UserQueries userQueries, UserCommands userCommands)
    {
        _userQueries = userQueries;
        _userCommands = userCommands;
    }

    /// <summary>
    /// Retrieves all users from the database, ordered by the last modified date in descending order.
    /// </summary>
    /// <returns>
    /// Returns an HTTP 200 OK response with a list of user objects if users are found.
    /// Returns an HTTP 404 Not Found response if no users exist in the database.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserInfo>>> Get()
    {
        var users = await _userQueries.GetUsers();
        if (users != null && users.Any())
        {
            return Ok(users);
        }
        return NotFound("No users found");
    }

    /// <summary>
    /// Retrieves a user data, using the specified unique identifier (Guid).
    /// </summary>
    /// <param name="userGuid">
    /// The unique identifier (Guid) of the user to retrieve. This is used to locate the user in the database.
    /// </param>
    /// <returns>
    /// Returns an HTTP 200 OK response with the user object if found.
    /// Returns an HTTP 404 Not Found response if no user exists with the specified Guid.
    /// </returns>
    [HttpGet("{userGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserInfo>> Get(Guid userGuid)
    {
        var user = await _userQueries.GetUser(userGuid);
        if (user != null)
        {
            return Ok(user);
        }
        return NotFound("User not found");
    }

    /// <summary>
    /// Saves a user to the database. This method ensures idempotence by using the provided Guid
    /// to avoid creating duplicate entries for the same user.
    /// </summary>
    /// <param name="guid">
    /// A unique identifier (Guid) provided by the client. This is used to ensure that the user
    /// is not duplicated in the database.
    /// </param>
    /// <param name="user">
    /// The user information to be saved. This includes details such as name, email, and other user-specific data.
    /// </param>
    /// <returns>
    /// Returns an HTTP 201 Created response with the Guid of the saved user if successful.
    /// Returns an HTTP 400 Bad Request response if the input is invalid.
    /// </returns>
    // POST api/<UsersV2Controller>
    [HttpPost]
    //[Route("post")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(Guid guid, [FromBody] UserInfo user)
    {
        if (user == null)
        {
            return BadRequest("User is null");
        }
        user.Guid = guid;
        await _userCommands.SaveUser(user);
        //await Task.Delay(3000); // Simulate a long-running task so test if obj will be save as dublicate
        return CreatedAtAction(nameof(Post), new { guid = user.Guid }, user);

    }


    /// <summary>
    /// Generates or retrieves a unique identifier (Guid) for a user.
    /// This method ensures idempotence by returning the same Guid if provided,
    /// and generating a new one only when no Guid is supplied.
    /// An example of how to use this endpoint is: entity/post/guid e.g. entity/post/1234-asasd-21312
    /// </summary>
    /// <param name="guid">
    /// An optional Guid provided by the client. If null, a new Guid is generated.
    /// </param>
    /// <returns>
    /// Returns an HTTP 200 response with the provided or newly generated Guid.
    /// </returns>
    ////Get: entity/post
    ////Get: entity/post/guid e.g. entity/post/1234-asasd-21312   
    [HttpGet]
    [Route("CreateGuid")]

    public IActionResult CreateGuid(Guid? guid)
    {
        // Generate a new Guid if none is provided
        var generatedGuid = guid ?? Guid.NewGuid();

        // Return the Guid directly
        return Ok(new { Guid = generatedGuid });
    }

    /// <summary>
    /// Updates a user's information based on the provided ID and JSON payload.
    /// </summary>
    /// <param name="id">The unique identifier (Guid) of the user to update.</param>
    /// <param name="userInfo">
    /// A JSON object containing the user's updated information.
    /// Example:
    /// <code>
    /// {
    ///   "FirstName": "John",
    ///   "LastName": "Doe",
    ///   "Email": "john.doe@example.com",
    ///   "PhoneNumber": "1234567890",
    ///   "UserName": "johndoe"
    /// }
    /// </code>
    /// </param>
    /// <returns>
    /// Returns an HTTP 200 OK response if the update is successful.
    /// Returns an HTTP 400 Bad Request response if the input is invalid.
    /// </returns>
    /// <remarks>
    /// Example Request:
    /// POST /User/Edit/123e4567-e89b-12d3-a456-426614174000
    /// {
    ///   "FirstName": "John",
    ///   "LastName": "Doe",
    ///   "Email": "john.doe@example.com",
    ///   "PhoneNumber": "1234567890",
    ///   "UserName": "johndoe"
    /// }
    /// </remarks>
    [HttpPost]
    [Route("Edit/{id}")]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [FromBody] JsonElement userInfo)
    {
        var userLastModifiedTicks = await _userQueries.GetUser(id);

        var firstName = userInfo.GetProperty("FirstName").GetString();
        var lastName = userInfo.GetProperty("LastName").GetString();
        var email = userInfo.GetProperty("Email").GetString();
        var phoneNumber = userInfo.GetProperty("PhoneNumber").GetString();
        var userName = userInfo.GetProperty("UserName").GetString();

        var user = new UserInfo
        {
            Guid = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            UserName = userName,
            LastModifiedTicks = userLastModifiedTicks.LastModifiedTicks
        };

        // Save the user to the database
        await _userCommands.SaveUser(user);
        // Return a success response
        return Ok(new { message = "User updated successfully" });  

    }

    [HttpPost]
    [Route("Delete/{guid}")]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid guid)
    {

        try
        {
            var userId = await _userQueries.GetUser(guid);
            await _userCommands.DeleteUser(userId.Guid);
            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error deleting user: {ex.Message}" });
        }

    }





    //[HttpGet("getstringFromUserService")]
    [HttpGet("getstringFromUserService")]

    public string GetString()
    {
        return "This is a string response from user";
    }
}
