using BlazorServer.Shared.Composition;

namespace BlazorServer.Shared.ModuleRegistration;

public sealed class ModuleRegistrator
{
    private readonly WebApplicationBuilder builder;
    private readonly HashSet<IModule> modules = [];
    private readonly AppRegistrationContext registrationContext;

    public ModuleRegistrator(WebApplicationBuilder builder, AppRegistrationContext registrationContext)
    {
        this.builder = builder;
        this.registrationContext = registrationContext;
    }

    public void AddModule<T>()
        where T : IModule, new()
    {
        var module = new T();
        modules.Add(module);
        module.Register(new ModuleRegistration(builder.Services, registrationContext, typeof(T)));
    }
}
