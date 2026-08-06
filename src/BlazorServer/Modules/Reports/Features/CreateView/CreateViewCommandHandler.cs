using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Persistence;

namespace BlazorServer.Modules.Reports.Features.CreateView;

internal sealed class CreateViewCommandHandler(
    IViewStore viewStore,
    CreateViewCommandValidator? validator = null)
    : ICommandHandler<CreateViewCommand, CreateViewResult>
{
    private readonly CreateViewCommandValidator _validator = validator ?? new CreateViewCommandValidator();

    public async Task<CreateViewResult> HandleAsync(
        CreateViewCommand command,
        CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidCreateViewRequest(errors);
        }

        var view = new View
        {
            Name = command.Name,
            OrganizationId = command.OrganizationId
        };

        await viewStore.UpsertAsync(view);

        return new ViewCreated(view.Id, view.OrganizationId, view.Name, view);
    }
}
