using BlazorServer.Modules.Projects.Features.CreateProject;
using BlazorServer.Modules.Projects.Infrastructure;
using BlazorServer.Modules.Projects.Infrastructure.InMemory;
using BlazorServer.Composition;

namespace BlazorServer.Modules.Projects;

public static class ProjectsModule
{
    public static IServiceCollection AddProjectsModule(
        this IServiceCollection services,
        AppRegistrationContext registrationContext)
    {
        services.AddSingleton<IProjectStore, InMemoryProjectStore>();
        services.AddTransient<CreateProjectCommandHandler>();

        return services;
    }
}
