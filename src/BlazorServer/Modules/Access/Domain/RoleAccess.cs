namespace BlazorServer.Modules.Access.Domain;
public class RoleAccess
{
    public RoleAccess(string roleId, string organizationId, IEnumerable<string> accessClaimValues)
    {
        if(string.IsNullOrWhiteSpace(roleId)) throw new ArgumentException("RoleId cannot be null or empty.", nameof(roleId));
        if(string.IsNullOrWhiteSpace(organizationId)) throw new ArgumentException("OrgId cannot be null or empty.", nameof(organizationId));
        if(accessClaimValues == null) throw new ArgumentNullException(nameof(accessClaimValues));

        RoleId = roleId;
        OrganizationId = organizationId;
        AccessClaimValues = accessClaimValues.ToList().AsReadOnly();
    }
    public string RoleId { get; }
    public string OrganizationId { get; }
    public IReadOnlyList<string> AccessClaimValues { get; }
}
