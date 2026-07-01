using BlazorServer.Shared.FeatureRegistration;

namespace BlazorServer.Shared.Composition;

public sealed class AppRegistrationContext
{
    public AppRegistrationContext()
    {
        RenderCollection = new RenderCollection(UiComposition);
    }

    public UiCompositionRegistry UiComposition { get; } = new();

    public RenderCollection RenderCollection { get; }

    public ClaimInfoCollection ClaimInfoCollection { get; } = new();

    public PolicyCollection Policies { get; } = new();
}
