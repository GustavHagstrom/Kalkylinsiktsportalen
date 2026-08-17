namespace BlazorServer.Modules.Reports.Domain.Views;

public sealed record SectionId
{
    public SectionId(string value)
    {
        Value = value.Trim();
    }

    public string Value { get; }
    public static SectionId New() => new(Guid.NewGuid().ToString());
    public static SectionId Empty => new(string.Empty);
}
