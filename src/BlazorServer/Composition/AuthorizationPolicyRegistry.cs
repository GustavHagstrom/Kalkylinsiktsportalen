using Microsoft.AspNetCore.Authorization;
namespace BlazorServer.Composition;

public class AuthorizationPolicyRegistry
{
    private readonly List<(string Name, Action<AuthorizationPolicyBuilder> Builder)> policies = [];

    public IReadOnlyList<(string Name, Action<AuthorizationPolicyBuilder> Builder)> PolicyList => policies;

    public void AddPolicy(string name, Action<AuthorizationPolicyBuilder> builder)
    {
        policies.Add((name, builder));
    }
}
