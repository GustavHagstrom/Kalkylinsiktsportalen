using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Domain.Views.SheetSection;

public class SheetSection
{
    public string SectionId { get; set; } = SectionIds.New();
    public int SectionIndex { get; set; } = 0;
    public string Content { get; set; } = string.Empty;
    public bool EnableChanges { get; set; } = false;
    /// <summary>
    /// Key: $"{rowType}-{columnType}"
    /// </summary>
    public Dictionary<string, SheetCell> SheetCells { get; set; } = new();

    public void SetEnableChanges(bool enableChanges)
    {
        EnableChanges = enableChanges;
    }

    public SheetCell GetCellConfiguration(RowType rowType, ColumnType columnType)
    {
        Normalize();
        var key = BuildCellKey(rowType, columnType);
        return SheetCells.TryGetValue(key, out var cell)
            ? cell
            : CreateDefaultCell(rowType, columnType);
    }

    public void ApplyFormatPatch(RowType rowType, ColumnType columnType, ReportFormatPatch patch)
    {
        ArgumentNullException.ThrowIfNull(patch);
        Normalize();

        var key = BuildCellKey(rowType, columnType);
        var cell = SheetCells.TryGetValue(key, out var existing)
            ? existing
            : CreateDefaultCell(rowType, columnType);

        cell.ApplyFormatPatch(patch);
        StoreOrPruneCell(key, cell);
    }

    public void Normalize()
    {
        if (string.IsNullOrWhiteSpace(SectionId))
            SectionId = SectionIds.New();

        SheetCells ??= new Dictionary<string, SheetCell>();

        var normalizedCells = new Dictionary<string, SheetCell>();
        foreach (var keyedCell in SheetCells)
        {
            var cell = keyedCell.Value;
            cell.ApplyLegacyFormatting();

            var key = BuildCellKey(cell.RowType, cell.ColumnType);
            if (!cell.IsDefault())
                normalizedCells[key] = cell;
        }

        SheetCells = normalizedCells;
    }

    private void StoreOrPruneCell(string key, SheetCell cell)
    {
        if (cell.IsDefault())
        {
            SheetCells.Remove(key);
            return;
        }

        SheetCells[key] = cell;
    }

    private static SheetCell CreateDefaultCell(RowType rowType, ColumnType columnType)
        => new()
        {
            RowType = rowType,
            ColumnType = columnType
        };

    private static string BuildCellKey(RowType rowType, ColumnType columnType)
        => $"{rowType}-{columnType}";
}
