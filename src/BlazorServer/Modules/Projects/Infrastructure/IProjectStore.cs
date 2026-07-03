using BlazorServer.Modules.Projects.Domain;

namespace BlazorServer.Modules.Projects.Infrastructure;

public interface IProjectStore
{
    Task AddAsync(Project project, CancellationToken cancellationToken);

    Task<Project?> GetAsync(ProjectId projectId, CancellationToken cancellationToken);
}
