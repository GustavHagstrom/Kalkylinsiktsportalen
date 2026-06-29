using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.Registration;

public sealed class AccessRegistration
{
    private readonly ClaimInfoCollection claimInfoCollection;

    public AccessRegistration(ClaimInfoCollection claimInfoCollection)
    {
        this.claimInfoCollection = claimInfoCollection;
    }

    public void DeclareClaim(string value, string name, string description)
    {
        claimInfoCollection.AddAccessClaimInfo(new AccessClaimInfo
        {
            Value = value,
            Name = name,
            Description = description
        });
    }
}
