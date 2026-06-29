namespace BlazorServer.Shared.Registration;

public sealed class ModuleRegistration
{
    public ModuleRegistration(IServiceCollection services, AppRegistrationContext registrationContext)
    {
        Services = services;
        Ui = new UiCompositionRegistration(registrationContext.UiComposition);
        Access = new AccessRegistration(registrationContext.ClaimInfoCollection);
        Authorization = new AuthorizationRegistration(registrationContext.Policies);
    }

    public IServiceCollection Services { get; }

    public UiCompositionRegistration Ui { get; }

    public AccessRegistration Access { get; }

    public AuthorizationRegistration Authorization { get; }
}
