namespace BlazorServer.Composition.Settings;

public sealed class SettingsAreaRegistry
{
    private readonly List<SettingsAreaEntry> settingsAreas = [];
    private int sequence;

    public IReadOnlyList<SettingsAreaContribution> SettingsAreas => Ordered(settingsAreas);

    public void Add(SettingsAreaContribution area, string moduleKey)
    {
        if (!area.Href.StartsWith("/settings/", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"{area.Href} is not a valid settings area href. Settings area hrefs must start with /settings/.");
        }

        if (settingsAreas.Any(registration => registration.Area.Id == area.Id))
        {
            throw new InvalidOperationException(
                $"{area.Id} is already registered for {nameof(SettingsAreas)}.");
        }

        if (settingsAreas.Any(registration => registration.ModuleKey == moduleKey))
        {
            throw new InvalidOperationException(
                $"{moduleKey} has already registered a settings area.");
        }

        settingsAreas.Add(new SettingsAreaEntry(area, moduleKey, sequence++));
    }

    private static SettingsAreaContribution[] Ordered(IEnumerable<SettingsAreaEntry> registrations)
    {
        return registrations
            .OrderBy(registration => registration.Area.Order)
            .ThenBy(registration => registration.Sequence)
            .Select(registration => registration.Area)
            .ToArray();
    }

    private sealed record SettingsAreaEntry(SettingsAreaContribution Area, string ModuleKey, int Sequence);
}
