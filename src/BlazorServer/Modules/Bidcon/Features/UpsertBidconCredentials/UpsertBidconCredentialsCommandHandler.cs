using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public sealed class UpsertBidconCredentialsCommandHandler
    : ICommandHandler<UpsertBidconCredentialsCommand, UpsertBidconCredentialsResult>
{
    private readonly IBidconApiCredentialsStore credentialsStore;
    private readonly IValidator<UpsertBidconCredentialsCommand> validator;

    public UpsertBidconCredentialsCommandHandler(
        IBidconApiCredentialsStore credentialsStore,
        IValidator<UpsertBidconCredentialsCommand>? validator = null)
    {
        this.credentialsStore = credentialsStore;
        this.validator = validator ?? new UpsertBidconCredentialsCommandValidator();
    }

    public async Task<CommandResult<UpsertBidconCredentialsResult>> HandleAsync(
        UpsertBidconCredentialsCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return CommandResult<UpsertBidconCredentialsResult>.Invalid(validationResult.Errors.ToArray());
        }

        await credentialsStore.UpsertAsync(new BidconApiCredentials
        {
            OrganizationId = command.OrganizationId.Value.ToString(),
            ApiKey = command.ApiKey,
            ApiSecret = command.ApiSecret
        });

        return CommandResult<UpsertBidconCredentialsResult>.Success(
            new UpsertBidconCredentialsResult(command.OrganizationId, true));
    }
}
