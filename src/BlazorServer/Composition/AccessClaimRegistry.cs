using BlazorServer.Modules.Access.Public.Contracts;

namespace BlazorServer.Composition;

public sealed class AccessClaimRegistry
{
    private readonly Dictionary<string, AccessClaimContribution> claimsByValue = new(StringComparer.Ordinal);
    private readonly List<AccessClaimContribution> claims = [];

    public IReadOnlyList<AccessClaimContribution> Claims => claims;

    public void Register(AccessClaimContribution claimInfo)
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
public sealed record AccessClaimContribution(string Value, string Name, string Description);
