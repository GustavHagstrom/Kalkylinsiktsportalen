using Core.Abstractions;

namespace SharedModule.Services;
public interface IApplicationRenderComponentsService
{
    IEnumerable<IInjectableComponent> CommonSettingsComponents { get; }
    IEnumerable<IInjectableComponent> SettingsNavigationComponents { get; }
    IEnumerable<IInjectableComponent> AppBarNavLinkComponents { get; }
}
