using Microsoft.AspNetCore.Authorization;

namespace BlazorServer.Shared.Registration;

public sealed class AuthorizationRegistration
{
    private readonly PolicyCollection policyCollection;

    public AuthorizationRegistration(PolicyCollection policyCollection)
    {
        this.policyCollection = policyCollection;
    }

    public void AddPolicy(string name, Action<AuthorizationPolicyBuilder> builder)
    {
        policyCollection.AddPolicy(name, builder);
    }
}
