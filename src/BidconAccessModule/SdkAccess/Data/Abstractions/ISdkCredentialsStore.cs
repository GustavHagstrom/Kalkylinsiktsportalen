using Core.Models;

namespace BidconAccessModule.SdkAccess.Data.Abstractions;
public interface ISdkCredentialsStore
{
    Task<SdkCredentials?> GetSdkCredentialsAsync(string tenantId);
    Task UpsertCredentialsAsync(string tenantId, SdkCredentials credentials);
}
