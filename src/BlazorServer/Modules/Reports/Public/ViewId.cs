namespace BlazorServer.Modules.Reports.Public;

public readonly record struct ViewId(string Value)
{
    public static ViewId New() => new(Guid.NewGuid().ToString());

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static implicit operator ViewId(string value) => new(value);

    public override string ToString() => Value;
}
