using Microsoft.AspNetCore.Mvc;

namespace BlazorServer.AppShell.Authentication;

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
