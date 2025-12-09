using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.MockedData;

public static class MockedDataStore
{
    public static List<User> Users = 
    [
        new User
        {
            Id = "Admin",
            Name = "Admin",
            AvatarBase64 = null
        },
        new User
        {
            Id = "User",
            Name = "User",
            AvatarBase64 = null
        },
        new User
        {
            Id = "Guest",
            Name = "Guest",
            AvatarBase64 = null
        }
    ];
    public static List<Role> Roles = 
    [
        new Role
        {
            Id = "Admin",
            Name = "AdminRole",
            Description = "Administrator role with full permissions."
        },
        new Role
        {
            Id = "User",
            Name = "UserRole",
            Description = "Standard user role with limited permissions."
        },
        new Role
        {
            Id = "Guest",
            Name = "GuestRole",
            Description = "Guest role with minimal permissions."
        }
    ];
    public const string OrganizationId = "MockedOrganization";
}
