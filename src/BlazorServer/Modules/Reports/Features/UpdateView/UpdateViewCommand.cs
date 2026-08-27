using BlazorServer.Kernel.CQRS;
using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSections;
using BlazorServer.Modules.Reports.Domain.Views.SheetSections;

namespace BlazorServer.Modules.Reports.Features.UpdateView;

public sealed record UpdateViewCommand(
    OrganizationId OrganizationId,
    ViewId ViewId,
    string Name,
    string Description,
    IReadOnlyList<IViewSection> Sections,
    bool ConfirmSectionDelete = false) : ICommand<UpdateViewResult>
{
    public static UpdateViewCommand FromEditedView(OrganizationId organizationId, View view)
        => new(
            organizationId,
            view.Id,
            view.Name,
            view.Description,
            view.Sections.ToArray());
}
