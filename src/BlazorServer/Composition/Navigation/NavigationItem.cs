using Microsoft.AspNetCore.Components.Routing;

namespace BlazorServer.Composition.Navigation;

public sealed record NavigationItem(
    string Text,
    string Href,
    int Order,
    string? Icon = null,
    string? Policy = null,
    NavLinkMatch Match = NavLinkMatch.Prefix);
