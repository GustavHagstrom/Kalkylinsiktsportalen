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
        if (user is not null)
        {
            return await GetUserPhotoAsync(user);
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
                return await GetUserPhotoAsync(user);
            });
            return await Task.WhenAll(tasks);
        }

        return Enumerable.Empty<UserWithPhoto>();
    }
    public async Task<User?> GetUserAsync(string id)
    {
        return await graphServiceClient.Users[id].GetAsync();
    }
    public async Task<UserWithPhoto?> GetUserWithPhotoAsync(string id)
    {
        var user = await GetUserAsync(id);
        if (user is not null)
        {
            return await GetUserPhotoAsync(user);
        }
        return null;
    }


    async Task<UserWithPhoto> GetUserPhotoAsync(User user)
    {
        var stream = await GetPhotoStreamAsync(user.Id);
        var bytes = ReadStream(stream);
        var photo = bytes is not null ? Convert.ToBase64String(bytes) : null;
        return new UserWithPhoto { User = user, PhotoBase64 = photo };
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
