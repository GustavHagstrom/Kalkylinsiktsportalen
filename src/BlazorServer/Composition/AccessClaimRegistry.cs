using BlazorServer.Modules.Access.Public.Contracts;

namespace BlazorServer.Composition;

public sealed class AccessClaimRegistry
{
    private readonly Dictionary<string, AccessClaimDefinition> claimsByValue = new(StringComparer.Ordinal);
    private readonly List<AccessClaimDefinition> claims = [];

    public IReadOnlyList<AccessClaimDefinition> Claims => claims;

    public void Register(AccessClaimDefinition claimInfo)
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
