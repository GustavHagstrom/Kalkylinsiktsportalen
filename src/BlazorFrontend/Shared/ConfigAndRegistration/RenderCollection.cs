using Microsoft.AspNetCore.Components;

namespace BlazorFrontend.Shared.ConfigAndRegistration;

public class RenderCollection
{
    private HashSet<Type> _appBarNavigationComponents = new();
    private HashSet<MenuLink> _createNewLinks = new();

    public void AddAppBarNavigationComponent<T>() where T : ComponentBase => _appBarNavigationComponents.Add(typeof(T));
    public IEnumerable<Type> GetAppBarNavigationComponents() => _appBarNavigationComponents;
    public void AddCreateNewLink(MenuLink link) => _createNewLinks.Add(link);
    public IEnumerable<MenuLink> GetCreateNewLinks() => _createNewLinks;
}
