using Core.Models.FromShared;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace SharedModule.Services.MicrosoftGraph;
internal class GraphClientUserAccess(GraphServiceClient graphServiceClient) : IMicrosoftGraphUserAccess
{
    public async Task<User?> GetMeAsync()
    {
        return await graphServiceClient.Me.GetAsync();
    }
    public async Task<UserWithPhoto?> GetMeWithPhotoAsync()
    {
        var user = await GetMeAsync();
        if (user is not null && user.Id is not null)
        {
            var photo = await GetUserPhotoAsync(user.Id);
            return new UserWithPhoto
            {
                User = user,
                PhotoBase64 = photo
            };
        }
        return null;
    }
    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        var users = await graphServiceClient.Users.GetAsync();
        return users?.Value ?? Enumerable.Empty<User>();
    }
    public async Task<IEnumerable<UserWithPhoto>> GetUsersWithPhotoAsync()
    {
        var users = await GetUsersAsync();
        if (users is not null)
        {
            var tasks = users.Select(async user =>
            {
                string? photo = null;
                if (user.Id is not null)
                {
                    photo = await GetUserPhotoAsync(user.Id);
                }
                return new UserWithPhoto
                {
                    User = user,
                    PhotoBase64 = photo
                };
            });
            return await Task.WhenAll(tasks);
        }

        return Enumerable.Empty<UserWithPhoto>();
    }
    public async Task<User?> GetUserAsync(string userId)
    {
        return await graphServiceClient.Users[userId].GetAsync();
    }
    public async Task<UserWithPhoto?> GetUserWithPhotoAsync(string userId)
    {
        var user = await GetUserAsync(userId);
        if (user is not null)
        {
            var photo = await GetUserPhotoAsync(userId);
            return new UserWithPhoto
            {
                User = user,
                PhotoBase64 = photo
            };
        }
        return null;
    }

    /// <summary>
    /// Base64 image string
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<string?> GetUserPhotoAsync(string userId)
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
            return await graphServiceClient.Users[userId].Photo.Content.GetAsync();
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
