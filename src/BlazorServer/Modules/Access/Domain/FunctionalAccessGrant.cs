using BlazorServer.Identity;

namespace BlazorServer.Modules.Access.Domain;

public sealed class FunctionalAccessGrant
{
    public FunctionalAccessGrant(
        FunctionalAccessGrantTarget target,
        OrganizationId organizationId,
        IEnumerable<string> accessClaimValues)
    {
        if (target is null) throw new ArgumentNullException(nameof(target));
        if (organizationId.IsEmpty) throw new ArgumentException("OrganizationId cannot be null or empty.", nameof(organizationId));
        if (accessClaimValues is null) throw new ArgumentNullException(nameof(accessClaimValues));

        Target = target;
        OrganizationId = organizationId;
        AccessClaimValues = accessClaimValues.ToList().AsReadOnly();
    }

    public FunctionalAccessGrantTarget Target { get; }
    public OrganizationId OrganizationId { get; }
    public IReadOnlyList<string> AccessClaimValues { get; }
}
