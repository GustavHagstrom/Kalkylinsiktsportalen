using BlazorServer.Kernel.CQRS;
using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Reports.Domain;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Persistence;

namespace BlazorServer.Modules.Reports.Features.UpdateView;

internal sealed class UpdateViewCommandHandler(
    IViewStore viewStore,
    UpdateViewCommandValidator? validator = null)
    : ICommandHandler<UpdateViewCommand, UpdateViewResult>
{
    private const string SectionsField = "Sections";
    private const string SectionDeleteConfirmationRequiredCode = "view.section_delete.confirmation_required";
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
            var deletedSectionIds = FindDeletedSectionIds(view, command);
            if (deletedSectionIds.Count > 0 && !command.ConfirmSectionDelete)
            {
                return new InvalidUpdateViewRequest(
                [
                    new ValidationError(
                        SectionDeleteConfirmationRequiredCode,
                        SectionsField,
                        "Deleting a section requires explicit confirmation because historical view-scoped changes can reference it.")
                ]);
            }

            view.ApplySectionEdits(command.DataSections, command.SheetSections);
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

    private static IReadOnlyList<string> FindDeletedSectionIds(
        View view,
        UpdateViewCommand command)
    {
        var editedSectionIds = command.DataSections
            .Select(section => section.SectionId)
            .Concat(command.SheetSections.Select(section => section.SectionId))
            .ToHashSet(StringComparer.Ordinal);

        return view.Sections
            .Select(section => section.SectionId)
            .Where(sectionId => !editedSectionIds.Contains(sectionId))
            .ToList();
    }
}
