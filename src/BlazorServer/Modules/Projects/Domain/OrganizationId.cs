namespace BlazorServer.Modules.Projects.Domain;

public readonly record struct OrganizationId(string Value)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
}
