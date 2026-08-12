using BlazorServer.Kernel.CQRS;
using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Reports.Domain;
using BlazorServer.Modules.Reports.Persistence;

namespace BlazorServer.Modules.Reports.Features.UpdateView;

internal sealed class UpdateViewCommandHandler(
    IViewStore viewStore,
    UpdateViewCommandValidator? validator = null)
    : ICommandHandler<UpdateViewCommand, UpdateViewResult>
{
    private const string SectionsField = "Sections";
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

        try
        {
            view.ApplyLegacyEditedSections(command.DataSections, command.SheetSections);
            view.Rename(command.Name);
            view.UpdateDescription(command.Description);
        }
        catch (ReportsDomainException exception)
        {
            return new InvalidUpdateViewRequest(
            [
                new ValidationError(
                    exception.Code,
                    SectionsField,
                    exception.Message)
            ]);
        }

        await viewStore.UpsertAsync(view);

        return new ViewUpdated(
            view.Id,
            view.OrganizationId,
            view.Name,
            view);
    }
}
