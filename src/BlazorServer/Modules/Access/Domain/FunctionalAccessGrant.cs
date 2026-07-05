namespace BlazorServer.Modules.Access.Domain;

public sealed class FunctionalAccessGrant
{
    public FunctionalAccessGrant(
        FunctionalAccessGrantTarget target,
        string organizationId,
        IEnumerable<string> accessClaimValues)
    {
        if (target is null) throw new ArgumentNullException(nameof(target));
        if (string.IsNullOrWhiteSpace(organizationId)) throw new ArgumentException("OrganizationId cannot be null or empty.", nameof(organizationId));
        if (accessClaimValues is null) throw new ArgumentNullException(nameof(accessClaimValues));

        Target = target;
        OrganizationId = organizationId;
        AccessClaimValues = accessClaimValues.ToList().AsReadOnly();
    }

    public FunctionalAccessGrantTarget Target { get; }
    public string OrganizationId { get; }
    public IReadOnlyList<string> AccessClaimValues { get; }
}
