namespace BlazorServer.Shared.Stores;

public interface IDarkmodeStore
{
    Task<bool> IsDarkmodeAsync(string userId);
    Task SetDarkmodeAsync(string userId, bool darkmode);
}
