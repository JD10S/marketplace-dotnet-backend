using Marketplace.Data.Interfaces;
using Marketplace.Entities.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User user)
    {
        _userService.Register(user);
        return Ok();
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User user)
    {
        var loggedUser = _userService.Login(user.Email, user.Password);
        return Ok(loggedUser);
    }
}
