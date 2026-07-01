namespace BlazorServer.Shared.ModuleRegistration;

public interface IModule
{
    void Register(ModuleRegistration module);
}
