using Microsoft.AspNetCore.Components;

namespace BlazorServer.Shared.FeatureRegistration;

public class RenderCollection
{
    private HashSet<Type> _appBarNavigationComponents = new();
    private Type? _loginDisplayComponent;
    public void AddAppBarNavigationComponent<T>() where T : ComponentBase => _appBarNavigationComponents.Add(typeof(T));
    public IEnumerable<Type> GetAppBarNavigationComponents() => _appBarNavigationComponents;
    public void SetLoginDisplayComponent<T>() where T : ComponentBase
    {
        _loginDisplayComponent = typeof(T);
    }
    public Type? GetLoginDisplayComponent() => _loginDisplayComponent;
}
