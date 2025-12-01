using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.Stores;

public interface IUserSettingsStore
{
    Task<UserSettings> GetByIdAsync(string userId);
    Task UpsertAsync(UserSettings model);
}
