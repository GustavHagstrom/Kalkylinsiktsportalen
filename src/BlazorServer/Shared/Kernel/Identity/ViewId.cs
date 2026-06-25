namespace BlazorServer.Shared.Kernel.Identity;

public readonly record struct ViewId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}
