using Microsoft.Graph;

namespace BlazorServer.Shared.Services.Authentication;

public class UserService : IUserService
{
    private readonly GraphServiceClient _graph;

    public UserService(GraphServiceClient graph)
    {
        _graph = graph;
    }
    public async Task<Models.User?> GetUserAsync(string userId)
    {
        var graphUser = await _graph.Users[userId].GetAsync();
        var photo = await GetUserPhotoAsync(userId);
        if (graphUser == null) return null;
        return new Models.User
        {
            Id = graphUser.Id!,
            Name = graphUser.DisplayName!,
            AvatarBase64 = photo
        };
    }

    public async Task<List<Models.User>> GetUsersAsync(string orgId)
    {
        var users = await _graph.Users.GetAsync();
        if (users?.Value is not null)
        {
            var tasks = users.Value.Select(async user =>
            {
                string? photo = null;
                if (user.Id is not null)
                {
                    photo = await GetUserPhotoAsync(user.Id);
                }
                return new Models.User
                {
                    Id = user.Id!,
                    Name = user.DisplayName!,
                    AvatarBase64 = photo
                };
            });
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }
        return [];
    }
    async Task<string?> GetUserPhotoAsync(string userId)
    {
        var stream = await GetPhotoStreamAsync(userId);
        var bytes = ReadStream(stream);
        var photo = bytes is not null ? Convert.ToBase64String(bytes) : null;
        return photo;
    }
    async Task<Stream?> GetPhotoStreamAsync(string? userId)
    {
        try
        {
            return await _graph.Users[userId].Photo.Content.GetAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }
    byte[]? ReadStream(Stream? input)
    {
        if (input is null) return null;
        try
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        catch (Exception)
        {
            return null;
        }

    }
}
