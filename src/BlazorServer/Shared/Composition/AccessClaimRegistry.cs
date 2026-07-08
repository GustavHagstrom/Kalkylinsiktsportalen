using BlazorServer.Modules.Access.Public.Contracts;

namespace BlazorServer.Shared.Composition;

public sealed class AccessClaimRegistry
{
    private readonly Dictionary<string, AccessClaimInfo> claimsByValue = new(StringComparer.Ordinal);
    private readonly List<AccessClaimInfo> claims = [];

    public IReadOnlyList<AccessClaimInfo> Claims => claims;

    public void Register(AccessClaimInfo claimInfo)
    {
        if (!claimsByValue.TryGetValue(claimInfo.Value, out var existingClaimInfo))
        {
            claimsByValue.Add(claimInfo.Value, claimInfo);
            claims.Add(claimInfo);
            return;
        }

        if (existingClaimInfo == claimInfo)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Access claim {claimInfo.Value} is already registered with different metadata.");
    }
}
