using Microsoft.AspNetCore.Components;

namespace BlazorServer.Shared.FeatureRegistration;

public class RenderCollection
{
    private HashSet<Type> _appBarNavigationComponents = new();
    private HashSet<Type> __sharedSettingsCompoenents = new();
    private HashSet<Type> _settingNavigationComponents = new();
    private Type? _loginDisplayComponent;
    public void AddAppBarNavigationComponent<T>() where T : ComponentBase => _appBarNavigationComponents.Add(typeof(T));
    public IEnumerable<Type> GetAppBarNavigationComponents() => _appBarNavigationComponents;

    /// <summary>
    /// Components to be rendered in the shared settings page
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void AddSharedSettingComponent<T>() where T : ComponentBase => __sharedSettingsCompoenents.Add(typeof(T));
    /// <summary>
    /// Returns a collection of types that represent shared setting components available in the current context.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="Type"/> objects, each representing a shared setting component. The
    /// collection may be empty if no shared setting components are defined. Used in the shared settings space to group settings from different features on the same page.</returns>
    public IEnumerable<Type> GetSharedSettingComponents() => __sharedSettingsCompoenents;
    public void AddSettingNavigationComponent<T>() where T : ComponentBase => _settingNavigationComponents.Add(typeof(T));
    public IEnumerable<Type> GetSettingNavigationComponents() => _settingNavigationComponents;

    public void SetLoginDisplayComponent<T>() where T : ComponentBase
    {
        _loginDisplayComponent = typeof(T);
    }
    public Type? GetLoginDisplayComponent() => _loginDisplayComponent;
}
