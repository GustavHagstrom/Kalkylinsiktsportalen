using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public interface IBidconCredentialsVerifier
{
    Task<bool> VerifyAsync(BidconApiCredentials credentials, CancellationToken cancellationToken);
}
