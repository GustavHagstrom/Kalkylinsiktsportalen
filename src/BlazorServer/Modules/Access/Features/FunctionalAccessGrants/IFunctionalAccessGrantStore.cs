using BlazorServer.Modules.Access.Domain;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public interface IFunctionalAccessGrantStore
{
    Task<FunctionalAccessGrant> GetAccessGrantAsync(FunctionalAccessGrantTarget target, string orgId);
    Task<List<AccessClaim>> GetAccessClaimsForUserAsync(string userId, IEnumerable<string> roleIds, string orgId);
    Task AddAccessGrantAsync(FunctionalAccessGrantTarget target, string orgId, string accessClaimValue);
    Task DeleteAccessGrantAsync(FunctionalAccessGrantTarget target, string orgId, string accessClaimValue);
}
