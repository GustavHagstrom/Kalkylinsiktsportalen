namespace BlazorServer.Modules.Reports.Domain.Views;

public sealed record SectionId
{
    public SectionId(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("SectionId cannot be null or whitespace.", nameof(value));
        }
        Value = value.Trim();
    }

    public string Value { get; }
    public static SectionId New() => new(Guid.NewGuid().ToString());
}
