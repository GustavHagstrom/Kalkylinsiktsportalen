namespace BlazorServer.Shared.Composition;

public sealed class AppRegistrationContext
{
    public UiCompositionRegistry UiComposition { get; } = new();

    public AccessClaimRegistry AccessClaims { get; } = new();

    public PolicyCollection Policies { get; } = new();
}
