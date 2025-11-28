using Microsoft.AspNetCore.Components.Routing;

namespace BlazorServer.Shared.FeatureRegistration;

public record MenuLink(string Title, string Route, string? Icon = null, NavLinkMatch? Match = null);
