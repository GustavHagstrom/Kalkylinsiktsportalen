namespace BlazorServer.Shared.Registration;

public sealed class ModuleRegistration
{
    public ModuleRegistration(IServiceCollection services, AppRegistrationContext registrationContext)
    {
        Services = services;
        AppShell = new AppShellRegistration(registrationContext.RenderCollection);
        Access = new AccessRegistration(registrationContext.ClaimInfoCollection);
        Authorization = new AuthorizationRegistration(registrationContext.Policies);
    }

    public IServiceCollection Services { get; }

    public AppShellRegistration AppShell { get; }

    public AccessRegistration Access { get; }

    public AuthorizationRegistration Authorization { get; }
}
