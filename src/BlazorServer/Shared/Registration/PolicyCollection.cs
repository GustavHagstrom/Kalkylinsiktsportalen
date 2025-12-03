using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BlazorServer.Shared.Registration;

public class PolicyCollection
{
    private List<(string Name, Action<AuthorizationPolicyBuilder> Builder)> _policies = new();
    public IReadOnlyList<(string Name, Action<AuthorizationPolicyBuilder> Builder)> PolicyList => _policies;
    public void AddPolicy(string name, Action<AuthorizationPolicyBuilder> builder)
    {
        _policies.Add((name, builder));
    }
}
