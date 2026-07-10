using BlazorServer.AppShell.Composition.UI;

namespace BlazorServer.AppShell.Composition;

public sealed class AppRegistrationContext
{
    public UiRegistry Ui { get; } = new();

    public AccessClaimRegistry AccessClaims { get; } = new();

    public PolicyCollection Policies { get; } = new();
}
