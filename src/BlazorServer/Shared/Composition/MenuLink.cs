using Microsoft.AspNetCore.Components.Routing;

namespace BlazorServer.Shared.Composition;

public record MenuLink(string Title, string Route, string? Icon = null, NavLinkMatch? Match = null, int Order = 0);
