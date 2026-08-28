namespace BlazorServer.Modules.Reports.Domain.Views;

public sealed class CellLayoutPatch
{
    public string? BackgroundColor { get; init; }
    public bool ClearBackgroundColor { get; init; }
    public double? PaddingLeftMm { get; init; }
    public bool ClearPaddingLeft { get; init; }
    public double? PaddingRightMm { get; init; }
    public bool ClearPaddingRight { get; init; }
    public double? PaddingTopMm { get; init; }
    public bool ClearPaddingTop { get; init; }
    public double? PaddingBottomMm { get; init; }
    public bool ClearPaddingBottom { get; init; }
    public CellBorder? LeftBorder { get; init; }
    public bool ClearLeftBorder { get; init; }
    public CellBorder? RightBorder { get; init; }
    public bool ClearRightBorder { get; init; }
    public CellBorder? TopBorder { get; init; }
    public bool ClearTopBorder { get; init; }
    public CellBorder? BottomBorder { get; init; }
    public bool ClearBottomBorder { get; init; }
}
