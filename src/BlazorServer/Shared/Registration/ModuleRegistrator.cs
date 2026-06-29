namespace BlazorServer.Shared.Registration;

public sealed class ModuleRegistrator
{
    private readonly WebApplicationBuilder builder;
    private readonly IServiceProvider serviceProvider;
    private readonly HashSet<IModule> modules = [];
    private readonly AppRegistrationContext registrationContext;

    public ModuleRegistrator(WebApplicationBuilder builder, AppRegistrationContext registrationContext)
    {
        this.builder = builder;
        this.registrationContext = registrationContext;
        serviceProvider = builder.Services.BuildServiceProvider();
    }

    public RenderCollection RenderCollection => registrationContext.RenderCollection;

    public ClaimInfoCollection ClaimInfoCollection => registrationContext.ClaimInfoCollection;

    public PolicyCollection Policies => registrationContext.Policies;

    public void AddModule<T>()
        where T : IModule
    {
        var module = ActivatorUtilities.CreateInstance<T>(serviceProvider);
        modules.Add(module);
        module.Register(new ModuleRegistration(builder.Services, registrationContext));
    }
}
