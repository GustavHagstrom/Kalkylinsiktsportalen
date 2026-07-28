using BlazorServer.Identity;
using BlazorServer.Modules.Access.Authentication;
using BlazorServer.Modules.Access.Domain;

namespace BlazorServer.Modules.Access.Persistence;

public interface IFunctionalAccessGrantStore
{
    Task<FunctionalAccessGrant> GetAccessGrantAsync(FunctionalAccessGrantTarget target, OrganizationId orgId);
    Task<List<AccessClaim>> GetAccessClaimsForUserAsync(UserId userId, IEnumerable<string> roleIds, OrganizationId orgId);
    Task AddAccessGrantAsync(FunctionalAccessGrantTarget target, OrganizationId orgId, string accessClaimValue);
    Task DeleteAccessGrantAsync(FunctionalAccessGrantTarget target, OrganizationId orgId, string accessClaimValue);
}
