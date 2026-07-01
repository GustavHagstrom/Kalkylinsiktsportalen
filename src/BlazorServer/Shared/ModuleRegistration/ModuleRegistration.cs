using BlazorServer.Shared.Composition;

namespace BlazorServer.Shared.ModuleRegistration;

public sealed class ModuleRegistration
{
    public ModuleRegistration(IServiceCollection services, AppRegistrationContext registrationContext)
        : this(services, registrationContext, Guid.NewGuid().ToString("N"))
    {
    }

    public ModuleRegistration(IServiceCollection services, AppRegistrationContext registrationContext, Type moduleType)
        : this(services, registrationContext, moduleType.FullName ?? moduleType.Name)
    {
    }

    private ModuleRegistration(IServiceCollection services, AppRegistrationContext registrationContext, string moduleKey)
    {
        Services = services;
        Ui = new UiCompositionRegistration(registrationContext.UiComposition, moduleKey);
        Access = new AccessRegistration(registrationContext.ClaimInfoCollection);
        Authorization = new AuthorizationRegistration(registrationContext.Policies);
    }

    public IServiceCollection Services { get; }

    public UiCompositionRegistration Ui { get; }

    public AccessRegistration Access { get; }

    public AuthorizationRegistration Authorization { get; }
}
