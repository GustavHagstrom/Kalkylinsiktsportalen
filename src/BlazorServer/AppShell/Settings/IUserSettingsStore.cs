namespace BlazorServer.AppShell.Settings;

public interface IUserSettingsStore
{
    Task<UserSettings> GetByIdAsync(string userId);
    Task UpsertAsync(UserSettings model);
}
