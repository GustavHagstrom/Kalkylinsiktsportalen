using BlazorServer.Shared.Composition;

namespace BlazorServer.Shared.ModuleRegistration;

public sealed class UiSettingsRegistration
{
    private readonly UiCompositionRegistry registry;
    private readonly string moduleKey;

    public UiSettingsRegistration(UiCompositionRegistry registry, string moduleKey)
    {
        this.registry = registry;
        this.moduleKey = moduleKey;
    }

    public void SetArea(
        string id,
        string title,
        string description,
        string href,
        int order,
        string? icon = null,
        string? policy = null)
    {
        registry.AddSettingsArea(
            new UiSettingsArea(id, title, description, href, order, icon, policy),
            moduleKey);
    }
}
