using BlazorServer.Modules.Projects.Domain;
using BlazorServer.Kernel.CQRS;
using BlazorServer.Identity;

namespace BlazorServer.Modules.Projects.Features.CreateProject;

public sealed record CreateProjectCommand(
    OrganizationId OrganizationId,
    ProjectId ProjectId,
    UserId CreatedBy,
    DateTimeOffset CreatedAt,
    string Name) : ICommand<CreateProjectResult>;
