namespace BlazorServer.Shared.Registration;

public sealed class UiCompositionRegistry
{
    private readonly List<UiNavigationEntry> topNavigation = [];
    private readonly List<UiNavigationEntry> settingsNavigation = [];
    private readonly List<UiComponentEntry> settingsSections = [];
    private int sequence;
    private Type? loginDisplayComponent;

    public IReadOnlyList<UiNavigationItem> TopNavigation => Ordered(topNavigation);

    public IReadOnlyList<UiNavigationItem> SettingsNavigation => Ordered(settingsNavigation);

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

    private sealed record UiNavigationEntry(UiNavigationItem Item, int Sequence);

    private sealed record UiComponentEntry(Type ComponentType, int Order, int Sequence);
}
