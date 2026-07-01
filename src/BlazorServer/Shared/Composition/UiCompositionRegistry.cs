namespace BlazorServer.Shared.Composition;

public sealed class UiCompositionRegistry
{
    private readonly List<UiNavigationEntry> topNavigation = [];
    private readonly List<UiNavigationEntry> settingsNavigation = [];
    private readonly List<UiSettingsAreaEntry> settingsAreas = [];
    private readonly List<UiComponentEntry> settingsSections = [];
    private int sequence;
    private Type? loginDisplayComponent;

    public IReadOnlyList<UiNavigationItem> TopNavigation => Ordered(topNavigation);

    public IReadOnlyList<UiNavigationItem> SettingsNavigation => Ordered(settingsNavigation);

    public IReadOnlyList<UiSettingsArea> SettingsAreas => Ordered(settingsAreas);

    public IReadOnlyList<UiComponentRegistration> SettingsSections => Ordered(settingsSections);

    public Type? LoginDisplayComponent => loginDisplayComponent;

    public void AddTopNavigation(UiNavigationItem item)
    {
        AddNavigation(topNavigation, item, nameof(TopNavigation));
    }

    public void AddSettingsNavigation(UiNavigationItem item)
    {
        AddNavigation(settingsNavigation, item, nameof(SettingsNavigation));
    }

    public void AddSettingsArea(UiSettingsArea area, string moduleKey)
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

        settingsAreas.Add(new UiSettingsAreaEntry(area, moduleKey, sequence++));
    }

    public void AddSettingsSection(Type componentType, int order)
    {
        AddComponent(settingsSections, componentType, order, nameof(SettingsSections));
    }

    public void SetLoginDisplay(Type componentType)
    {
        if (loginDisplayComponent is not null && loginDisplayComponent != componentType)
        {
            throw new InvalidOperationException(
                $"Login display component is already registered as {loginDisplayComponent.FullName}.");
        }

        loginDisplayComponent = componentType;
    }

    private void AddNavigation(
        List<UiNavigationEntry> registrations,
        UiNavigationItem item,
        string extensionPoint)
    {
        if (registrations.Any(registration => registration.Item.Href == item.Href))
        {
            throw new InvalidOperationException(
                $"{item.Href} is already registered for {extensionPoint}.");
        }

        registrations.Add(new UiNavigationEntry(item, sequence++));
    }

    private void AddComponent(
        List<UiComponentEntry> registrations,
        Type componentType,
        int order,
        string extensionPoint)
    {
        if (registrations.Any(registration => registration.ComponentType == componentType))
        {
            throw new InvalidOperationException(
                $"{componentType.FullName} is already registered for {extensionPoint}.");
        }

        registrations.Add(new UiComponentEntry(componentType, order, sequence++));
    }

    private static UiNavigationItem[] Ordered(IEnumerable<UiNavigationEntry> registrations)
    {
        return registrations
            .OrderBy(registration => registration.Item.Order)
            .ThenBy(registration => registration.Sequence)
            .Select(registration => registration.Item)
            .ToArray();
    }

    private static UiComponentRegistration[] Ordered(IEnumerable<UiComponentEntry> registrations)
    {
        return registrations
            .OrderBy(registration => registration.Order)
            .ThenBy(registration => registration.Sequence)
            .Select(registration => new UiComponentRegistration(
                registration.ComponentType,
                registration.Order))
            .ToArray();
    }

    private static UiSettingsArea[] Ordered(IEnumerable<UiSettingsAreaEntry> registrations)
    {
        return registrations
            .OrderBy(registration => registration.Area.Order)
            .ThenBy(registration => registration.Sequence)
            .Select(registration => registration.Area)
            .ToArray();
    }

    private sealed record UiNavigationEntry(UiNavigationItem Item, int Sequence);

    private sealed record UiSettingsAreaEntry(UiSettingsArea Area, string ModuleKey, int Sequence);

    private sealed record UiComponentEntry(Type ComponentType, int Order, int Sequence);
}
