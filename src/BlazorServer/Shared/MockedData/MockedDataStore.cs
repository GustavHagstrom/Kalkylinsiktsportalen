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
            Id = "RoleIDAdmin",
            Name = "Admin",
            Description = "Administrator role with full permissions."
        },
        new Role
        {
            Id = "RoleIDUser",
            Name = "User",
            Description = "Standard user role with limited permissions."
        },
        new Role
        {
            Id = "RoleIDGuest",
            Name = "Guest",
            Description = "Guest role with minimal permissions."
        }
    ];
}
