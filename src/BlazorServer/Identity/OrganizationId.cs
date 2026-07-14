namespace BlazorServer.Identity;

public readonly record struct OrganizationId(string Value)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
}
