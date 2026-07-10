namespace BlazorServer.Modules.Bidcon.Public.Contracts;

public readonly record struct OrganizationId(string Value)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
}
