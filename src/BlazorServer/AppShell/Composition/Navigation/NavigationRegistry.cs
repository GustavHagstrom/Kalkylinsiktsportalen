namespace BlazorServer.AppShell.Composition.Navigation;

public sealed class NavigationRegistry
{
    private readonly List<NavigationEntry> topNavigation = [];
    private readonly List<NavigationEntry> settingsNavigation = [];
    private int sequence;

    public IReadOnlyList<NavigationItem> TopNavigation => Ordered(topNavigation);

    /// <summary>
    /// Compatibility data for old IFeature settings navigation. The Settings Hub does not consume it.
    /// </summary>
    public IReadOnlyList<NavigationItem> SettingsNavigation => Ordered(settingsNavigation);

    public void AddTopNavigation(NavigationItem item)
    {
        AddNavigation(topNavigation, item, nameof(TopNavigation));
    }

    /// <summary>
    /// Registers compatibility settings navigation for old IFeature code.
    /// New module settings should register a Settings Area instead.
    /// </summary>
    public void AddSettingsNavigation(NavigationItem item)
    {
        AddNavigation(settingsNavigation, item, nameof(SettingsNavigation));
    }

    private void AddNavigation(
        List<NavigationEntry> registrations,
        NavigationItem item,
        string extensionPoint)
    {
        if (registrations.Any(registration => registration.Item.Href == item.Href))
        {
            throw new InvalidOperationException(
                $"{item.Href} is already registered for {extensionPoint}.");
        }

        registrations.Add(new NavigationEntry(item, sequence++));
    }

    private static NavigationItem[] Ordered(IEnumerable<NavigationEntry> registrations)
    {
        return registrations
            .OrderBy(registration => registration.Item.Order)
            .ThenBy(registration => registration.Sequence)
            .Select(registration => registration.Item)
            .ToArray();
    }

    private sealed record NavigationEntry(NavigationItem Item, int Sequence);
}
