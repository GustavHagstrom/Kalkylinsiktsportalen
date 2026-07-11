using BlazorServer.Modules.Access.Infrastructure.Identity;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class AccessDirectoryTests
{
    private const string OrganizationId = "org-1";

    [Test]
    public async Task MockAccessDirectoryReturnsIsolatedUserLists()
    {
        var directory = new MockAccessDirectory(OrganizationId);

        var firstRead = await directory.GetUsersAsync(OrganizationId);
        firstRead.Clear();

        var secondRead = await directory.GetUsersAsync(OrganizationId);

        Assert.That(secondRead.Select(user => user.Id), Is.EquivalentTo(["Admin", "User", "Guest"]));
    }

    [Test]
    public async Task MockAccessDirectoryResolvesRoleMemberships()
    {
        var directory = new MockAccessDirectory(OrganizationId);

        var adminRoles = await directory.GetRolesForUserAsync("Admin", OrganizationId);
        var adminUsers = await directory.GetUsersInRoleAsync("Admin", OrganizationId);

        Assert.Multiple(() =>
        {
            Assert.That(adminRoles.Select(role => role.Id), Has.One.EqualTo("Admin"));
            Assert.That(adminUsers.Select(user => user.Id), Has.One.EqualTo("Admin"));
        });
    }

    [Test]
    public async Task MockAccessDirectoryDoesNotReturnUsersForAnotherOrganization()
    {
        var directory = new MockAccessDirectory(OrganizationId);

        var users = await directory.GetUsersAsync("other-org");

        Assert.That(users, Is.Empty);
    }
}
