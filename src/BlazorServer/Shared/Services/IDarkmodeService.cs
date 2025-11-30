namespace BlazorServer.Shared.Services;

public interface IDarkmodeService
{
    Task<bool> GetAsync(string userId);
    Task SetAsync(string userId, bool darkmode);
}
