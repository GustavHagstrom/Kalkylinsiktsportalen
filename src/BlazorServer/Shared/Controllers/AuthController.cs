using BlazorServer.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorServer.Shared.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    [HttpGet("login")]
    public IActionResult Login()
    {
        return Redirect("/MicrosoftIdentity/Account/SignIn");
    }
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        return Redirect("/MicrosoftIdentity/Account/SignOut");
    }
    //[HttpGet("mock-login")]
    //public IActionResult MockLogin(User user)
    //{
    //    // Simulate a successful login by setting a mock user
    //    var mockUser = new User
    //    {
    //        Id = Guid.NewGuid(),
    //        Email = "mockuser@example.com",
    //        Name = "Mock User"
    //    };
    //    _session.SetCurrentUser(mockUser);
    //    return Ok();
    //}
}
