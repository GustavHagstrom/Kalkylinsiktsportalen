using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.Services.Authentication;

public class MockedAuthSession
{
    public User? CurrentUser { get; set; } 
}
