using BlazorServer.Modules.Projects.Domain;
using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Identity;

namespace BlazorServer.Modules.Projects.Features.CreateProject;

public sealed record CreateProjectCommand(
    OrganizationId OrganizationId,
    ProjectId ProjectId,
    UserId CreatedBy,
    DateTimeOffset CreatedAt,
    string Name) : ICommand<CreateProjectResult>;
