namespace BlazorServer.Shared.Kernel.Identity;

public readonly record struct OrganizationId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}
