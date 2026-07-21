namespace BlazorServer.AppShell.Authentication;

public sealed record LoginDisplayRecord
{
    public Type ComponentType { get; init; }

    private LoginDisplayRecord(Type loginDisplayComponent)
    {
        ComponentType = loginDisplayComponent;
    }
    public static LoginDisplayRecord Create<T>()
        where T : Microsoft.AspNetCore.Components.ComponentBase
    {
        return new LoginDisplayRecord(typeof(T));
    }
}
