using Core.Abstractions;
using Core.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace SharedModule.Services;
public class ApplicationRenderComponentsService : IApplicationRenderComponentsService
{
    private readonly IServiceProvider serviceProvider;

    public ApplicationRenderComponentsService(IServiceProvider sp)
    {
        serviceProvider = sp;
    }
    public IEnumerable<IInjectableComponent> CommonSettingsComponents => serviceProvider.GetKeyedServices<IInjectableComponent>(SharedInjectableComponentKeys.CommonSettingsComponent);

    public IEnumerable<IInjectableComponent> SettingsNavigationComponents => serviceProvider.GetKeyedServices<IInjectableComponent>(SharedInjectableComponentKeys.SettingsNavigationComponent);
    public IEnumerable<IInjectableComponent> AppBarNavLinkComponents => serviceProvider.GetKeyedServices<IInjectableComponent>(SharedInjectableComponentKeys.AppBarNavLinkComponent);
}
