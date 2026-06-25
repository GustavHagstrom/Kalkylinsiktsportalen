using BlazorServer.Modules.Projects.Domain;
using BlazorServer.Shared.Kernel.Identity;

namespace BlazorServer.Modules.Projects.Features.CreateProject;

public sealed record CreateProjectResult(ProjectId ProjectId, OrganizationId OrganizationId, string Name);
