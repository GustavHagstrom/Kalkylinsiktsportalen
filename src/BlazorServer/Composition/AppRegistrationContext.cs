using BlazorServer.Composition.UI;

namespace BlazorServer.Composition;

public sealed class AppRegistrationContext
{
    public UiRegistry Ui { get; } = new();

    public AccessClaimRegistry AccessClaims { get; } = new();

    public AuthorizationPolicyRegistry Policies { get; } = new();
}
