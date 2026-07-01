using BlazorServer.Modules.Projects.Features.CreateProject;
using BlazorServer.Modules.Projects.Infrastructure.InMemory;
using BlazorServer.Shared.Composition;
using BlazorServer.Shared.FeatureRegistration;
using BlazorServer.Shared.ModuleRegistration;

namespace BlazorServer.Modules.Projects;

public sealed class ProjectsModule : IModule
{
    public void Register(ModuleRegistration module)
    {
        module.Services.AddSingleton<IProjectStore, InMemoryProjectStore>();
        module.Services.AddTransient<CreateProjectCommandHandler>();
    }
}
