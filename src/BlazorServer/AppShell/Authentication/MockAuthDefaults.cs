namespace BlazorServer.AppShell.Authentication;

public class MockAuthDefaults
{
    public MockAuthDefaults(string organizationId)
    {
        OrganizationId = organizationId;
    }

    public string OrganizationId { get; }

    public IReadOnlyList<SessionUser> Users { get; } =
    [
        new("Admin", "Admin", "gustav@rorvikshus.se", null),
        new("User", "User", "gustav@rorvikshus.se", null),
        new("Guest", "Guest", "gustav@rorvikshus.se", null)
    ];

    public IReadOnlyDictionary<string, IReadOnlyList<string>> RoleIdsByUserId { get; } =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal)
        {
            ["Admin"] = ["Admin"],
            ["User"] = ["User"],
            ["Guest"] = ["Guest"]
        };
}
