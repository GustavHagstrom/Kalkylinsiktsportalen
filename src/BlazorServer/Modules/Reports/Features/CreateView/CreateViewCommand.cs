using BlazorServer.Identity;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Reports.Features.CreateView;

public sealed record CreateViewCommand(OrganizationId OrganizationId, string Name) : ICommand<CreateViewResult>;
