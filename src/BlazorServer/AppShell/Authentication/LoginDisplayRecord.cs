namespace BlazorServer.AppShell.Authentication;

public sealed record LoginDisplayRecord
{
    public Type Type { get; init; }

    private LoginDisplayRecord(Type loginDisplayComponent)
    {
        Type = loginDisplayComponent;
    }
    public static LoginDisplayRecord Create<T>()
        where T : Microsoft.AspNetCore.Components.ComponentBase
    {
        return new LoginDisplayRecord(typeof(T));
    }
}
