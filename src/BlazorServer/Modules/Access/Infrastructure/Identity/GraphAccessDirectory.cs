using BlazorServer.Modules.Access.Public.Contracts;
using Microsoft.Graph;

namespace BlazorServer.Modules.Access.Infrastructure.Identity;

public class GraphAccessDirectory : IAccessDirectory
{
    private readonly GraphServiceClient graphClient;

    public GraphAccessDirectory(GraphServiceClient graphClient)
    {
        this.graphClient = graphClient;
    }

    public async Task<AccessDirectoryUser?> GetUserAsync(string userId)
    {
        var graphUser = await graphClient.Users[userId].GetAsync();
        if (graphUser is null)
        {
            return null;
        }

        return new AccessDirectoryUser(
            graphUser.Id ?? string.Empty,
            graphUser.DisplayName ?? string.Empty,
            await GetUserPhotoAsync(userId));
    }

    public async Task<List<AccessDirectoryUser>> GetUsersAsync(string organizationId)
    {
        var users = await graphClient.Users.GetAsync();
        if (users?.Value is null)
        {
            return [];
        }

        var tasks = users.Value.Select(async user => new AccessDirectoryUser(
            user.Id ?? string.Empty,
            user.DisplayName ?? string.Empty,
            user.Id is null ? null : await GetUserPhotoAsync(user.Id)));

        return [.. await Task.WhenAll(tasks)];
    }

    public async Task<AccessDirectoryRole?> GetRoleAsync(string roleId)
    {
        var group = await graphClient.Groups[roleId].GetAsync();
        return group is null
            ? null
            : new AccessDirectoryRole(
                group.Id ?? string.Empty,
                group.DisplayName ?? string.Empty,
                group.Description ?? string.Empty);
    }

    public async Task<List<AccessDirectoryRole>> GetRolesAsync(string organizationId)
    {
        var groupsResponse = await graphClient.Groups.GetAsync();
        return groupsResponse?.Value?
            .Select(group => new AccessDirectoryRole(
                group.Id ?? string.Empty,
                group.DisplayName ?? string.Empty,
                group.Description ?? string.Empty))
            .ToList() ?? [];
    }

    public async Task<List<AccessDirectoryRole>> GetRolesForUserAsync(string userId, string organizationId)
    {
        var response = await graphClient.Users[userId].MemberOf.GetAsync();
        if (response?.Value is null)
        {
            return [];
        }

        List<AccessDirectoryRole> roles = [];
        foreach (var item in response.Value)
        {
            if (item is Microsoft.Graph.Models.Group group)
            {
                roles.Add(new AccessDirectoryRole(
                    group.Id ?? string.Empty,
                    group.DisplayName ?? string.Empty,
                    group.Description ?? string.Empty));
            }
        }

        return roles;
    }

    public async Task<List<AccessDirectoryUser>> GetUsersInRoleAsync(string roleId, string organizationId)
    {
        var members = await graphClient.Groups[roleId].Members.GetAsync();
        List<string> userIds = [];
        foreach (var member in members?.Value ?? [])
        {
            if (member is Microsoft.Graph.Models.User user && user.Id is not null)
            {
                userIds.Add(user.Id);
            }
        }

        var allUsers = await GetUsersAsync(organizationId);
        return allUsers.Where(user => userIds.Contains(user.Id)).ToList();
    }

    private async Task<string?> GetUserPhotoAsync(string userId)
    {
        var stream = await GetPhotoStreamAsync(userId);
        var bytes = ReadStream(stream);
        return bytes is not null ? Convert.ToBase64String(bytes) : null;
    }

    private async Task<Stream?> GetPhotoStreamAsync(string? userId)
    {
        try
        {
            return await graphClient.Users[userId].Photo.Content.GetAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static byte[]? ReadStream(Stream? input)
    {
        if (input is null)
        {
            return null;
        }

        try
        {
            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            return ms.ToArray();
        }
        catch (Exception)
        {
            return null;
        }
    }
}
