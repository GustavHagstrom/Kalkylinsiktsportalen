using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Reports.Views;

namespace BlazorServer.Modules.Reports.Features.UpdateView;

internal sealed class UpdateViewCommandHandler(
    IViewStore viewStore,
    UpdateViewCommandValidator? validator = null)
    : ICommandHandler<UpdateViewCommand, UpdateViewResult>
{
    private readonly UpdateViewCommandValidator _validator = validator ?? new UpdateViewCommandValidator();

    public async Task<UpdateViewResult> HandleAsync(
        UpdateViewCommand command,
        CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidUpdateViewRequest(errors);
        }

        var view = await viewStore.GetAsync(command.ViewId);
        if (view is null)
        {
            return new ViewToUpdateNotFound(command.ViewId);
        }

        if (view.OrganizationId != command.OrganizationId)
        {
            return new ViewUpdateOrganizationMismatch(command.ViewId, command.OrganizationId);
        }

        view.Name = command.Name;
        view.Description = command.Description;
        view.DataSections = command.DataSections.ToList();
        view.SheetSections = command.SheetSections.ToList();
        view.PageBreakSections = command.PageBreakSections.ToList();

        await viewStore.UpsertAsync(view);

        return new ViewUpdated(
            view.Id,
            view.OrganizationId,
            view.Name,
            view);
    }
}
