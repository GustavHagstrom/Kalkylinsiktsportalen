namespace BlazorServer.Modules.Reports.Domain.Views;

public sealed class TextStylePatch
{
    public ReportFont? Font { get; init; }
    public bool ClearFont { get; init; }
    public string? TextColor { get; init; }
    public bool ClearTextColor { get; init; }
    public double? FontSizePt { get; init; }
    public bool ClearFontSize { get; init; }
    public bool? IsBold { get; init; }
    public bool? IsItalic { get; init; }
    public bool? IsUnderline { get; init; }
    public HorizontalTextAlignment? HorizontalAlignment { get; init; }
    public bool ClearHorizontalAlignment { get; init; }
    public VerticalTextAlignment? VerticalAlignment { get; init; }
    public bool ClearVerticalAlignment { get; init; }
}
