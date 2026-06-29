using Microsoft.AspNetCore.Components.Routing;

namespace BlazorServer.Shared.Registration;

public sealed record UiNavigationItem(
    string Text,
    string Href,
    int Order,
    string? Icon = null,
    string? Policy = null,
    NavLinkMatch Match = NavLinkMatch.Prefix);
