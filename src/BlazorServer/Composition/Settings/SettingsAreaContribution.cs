namespace BlazorServer.Composition.Settings;

public sealed record SettingsAreaContribution(
    string Id,
    string Title,
    string Description,
    string Href,
    int Order,
    string? Icon = null,
    string? Policy = null);
