namespace BlazorServer.Shared.Composition;

public sealed record UiSettingsArea(
    string Id,
    string Title,
    string Description,
    string Href,
    int Order,
    string? Icon = null,
    string? Policy = null);
