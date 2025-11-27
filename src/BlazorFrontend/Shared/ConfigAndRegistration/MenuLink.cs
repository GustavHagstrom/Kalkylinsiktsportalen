using Microsoft.AspNetCore.Components.Routing;

namespace BlazorFrontend.Shared.ConfigAndRegistration;

public record MenuLink(string Title, string Route, string? Icon = null, NavLinkMatch? Match = null);
