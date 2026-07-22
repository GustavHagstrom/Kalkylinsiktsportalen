using BlazorServer.Identity;

namespace BlazorServer.AppShell.Authentication.Mock;

public class MockAuthDefaults
{
    public MockAuthDefaults(string organizationId)
    {
        OrganizationId = organizationId;
    }

    public string OrganizationId { get; }

    public IReadOnlyList<CurrentUser> Users =>
    [
        new(new("Admin"), new(OrganizationId), "Admin", "gustav@rorvikshus.se", null),
        new(new("User"), new(OrganizationId), "User", "gustav@rorvikshus.se", null),
        new(new("Guest"), new(OrganizationId), "Guest", "gustav@rorvikshus.se", null)
    ];

    public IReadOnlyDictionary<string, IReadOnlyList<string>> RoleIdsByUserId { get; } =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal)
        {
            ["Admin"] = ["Admin"],
            ["User"] = ["User"],
            ["Guest"] = ["Guest"]
        };
}
