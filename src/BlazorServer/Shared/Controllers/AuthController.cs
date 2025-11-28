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
}
