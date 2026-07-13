using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Public.Authorization;

public interface IViewAccessDecisionService
{
    Task<bool> CanUseViewAsync(AccessSubject subject, ViewId viewId);

    Task<bool> CanEditViewAsync(AccessSubject subject, ViewId viewId);

    Task<bool> CanAdministerViewAccessAsync(AccessSubject subject, ViewId viewId);
}
