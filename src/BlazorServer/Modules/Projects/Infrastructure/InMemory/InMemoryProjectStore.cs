using BlazorServer.Modules.Projects.Domain;

namespace BlazorServer.Modules.Projects.Infrastructure.InMemory;

public sealed class InMemoryProjectStore : IProjectStore
{
    private readonly Dictionary<ProjectId, Project> projects = [];

    public Task AddAsync(Project project, CancellationToken cancellationToken)
    {
        projects.Add(project.Id, project);

        return Task.CompletedTask;
    }

    public Task<Project?> GetAsync(ProjectId projectId, CancellationToken cancellationToken)
    {
        projects.TryGetValue(projectId, out var project);

        return Task.FromResult(project);
    }
}
