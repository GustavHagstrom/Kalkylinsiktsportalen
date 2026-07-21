using BlazorServer.Identity;
using Microsoft.Graph;

namespace BlazorServer.AppShell.Authentication;

public class GraphCurrentUserProfileLookup : ICurrentUserProfileLookup
{
    private readonly GraphServiceClient graph;

    public GraphCurrentUserProfileLookup(GraphServiceClient graph)
    {
        this.graph = graph;
    }

    public async Task<CurrentUser> GetProfileAsync(CurrentUser currentUser)
    {
        var graphUser = await graph.Users[currentUser.UserId.Value].GetAsync();
        return new CurrentUser(
            currentUser.UserId,
            currentUser.OrganizationId,
            graphUser?.DisplayName ?? currentUser.DisplayName,
            currentUser.Email,
            await GetUserPhotoAsync(currentUser.UserId.Value));
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
            return await graph.Users[userId].Photo.Content.GetAsync();
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
