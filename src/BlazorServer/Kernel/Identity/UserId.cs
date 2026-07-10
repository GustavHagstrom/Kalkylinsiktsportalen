namespace BlazorServer.Kernel.Identity;

public readonly record struct UserId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}
