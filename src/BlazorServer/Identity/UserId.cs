namespace BlazorServer.Identity;

public readonly record struct UserId(string Value)
{
    public bool IsEmpty => string.IsNullOrEmpty(Value);
}
