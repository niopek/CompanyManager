using CompanyManager.API.Library.Services;
using Microsoft.AspNetCore.Mvc;

namespace CompanyManager.API.Controllers;

public class UserController : Controller
{
    private readonly UsersService _userService;

    public UserController(UsersService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        try
        {
            var result = await _userService.CreateUser(request.Username, request.Email, request.Password);
            if(result == 1)
                return Ok("User created successfully.");
            else
                return BadRequest("User already exists.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.ValidateLogin(request.Username, request.Password);

        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(user);
    }
}

public class RegisterUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}