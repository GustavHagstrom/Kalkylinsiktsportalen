namespace BlazorServer.Shared.Registration;

public sealed class AppRegistrationContext
{
    public RenderCollection RenderCollection { get; } = new();

    public ClaimInfoCollection ClaimInfoCollection { get; } = new();

    public PolicyCollection Policies { get; } = new();
}
