using Microsoft.AspNetCore.Components.Routing;

namespace BlazorServer.Shared.Registration;

public record MenuLink(string Title, string Route, string? Icon = null, NavLinkMatch? Match = null);
