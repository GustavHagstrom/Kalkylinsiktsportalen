using BlazorServer.Identity;
using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Features.UpdateView;

public abstract record UpdateViewResult;

public sealed record ViewUpdated(
    ViewId ViewId,
    OrganizationId OrganizationId,
    string Name,
    View View) : UpdateViewResult;

public sealed record ViewToUpdateNotFound(ViewId ViewId) : UpdateViewResult;

public sealed record ViewUpdateOrganizationMismatch(
    ViewId ViewId,
    OrganizationId RequestedOrganizationId) : UpdateViewResult;

public sealed record InvalidUpdateViewRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : UpdateViewResult;
