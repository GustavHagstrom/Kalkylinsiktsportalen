using BlazorServer.Shared.Models;

namespace BlazorServer.Modules.Access.Domain;
public class UserAccess
{
    public UserAccess(string userId, string orgId, IEnumerable<string> accessClaimValues)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(orgId)) throw new ArgumentException("OrgId cannot be null or empty.", nameof(orgId));
        if (accessClaimValues == null) throw new ArgumentNullException(nameof(accessClaimValues));
        UserId = userId;
        OrganizationId = orgId;
        AccessClaimValues = accessClaimValues.ToList().AsReadOnly();
    }
    public string UserId { get; }
    public string OrganizationId { get; }
    public IReadOnlyList<string> AccessClaimValues { get; }
}
