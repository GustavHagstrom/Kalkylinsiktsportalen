using Microsoft.JSInterop;

namespace SharedModule.Services;
public class LocalStorageService(IJSRuntime JSRuntime) : ILocalStorageService
{

    public async Task<string?> GetItemAsync(string key)
    {
        return await JSRuntime.InvokeAsync<string>("eval", $"localStorage.getItem('{key}')");
    }

    public async Task SetItemAsync(string key, string value)
    {
        await JSRuntime.InvokeVoidAsync("eval", $"localStorage.setItem('{key}', '{value}')");
    }

    public async Task RemoveItemAsync(string key)
    {
        await JSRuntime.InvokeVoidAsync("eval", $"localStorage.removeItem('{key}')");
    }
}
