using BlazorServer.Identity;
using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Features.CreateView;

public abstract record CreateViewResult;

public sealed record ViewCreated(
    ViewId ViewId,
    OrganizationId OrganizationId,
    string Name,
    View View) : CreateViewResult;

public sealed record InvalidCreateViewRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : CreateViewResult;
