namespace BlazorServer.Modules.Reports.Domain.Views.SheetSections;

public class SheetSection : IViewSection
{
    public SectionId SectionId { get; private set; } = SectionId.New();
    public SheetSourceType SourceType { get; set; } = SheetSourceType.TenderSheet;
    public bool EnableChanges { get; set; } = false;
    public Dictionary<SheetCellKey, SheetCell> SheetCells { get; set; } = new();
    public PageBreakBehavior PageBreakBehavior { get; set; } = PageBreakBehavior.None;

    internal static SheetSection Reconstitute(SectionId sectionId, SheetSourceType sourceType = SheetSourceType.TenderSheet, bool enableChanges = false, IReadOnlyDictionary<SheetCellKey, SheetCell>? sheetCells = null, PageBreakBehavior pageBreakBehavior = PageBreakBehavior.None)
    {
        ArgumentNullException.ThrowIfNull(sectionId);

        var section = new SheetSection
        {
            SectionId = sectionId,
            SourceType = sourceType,
            EnableChanges = enableChanges,
            PageBreakBehavior = pageBreakBehavior
        };

        foreach (var kvp in sheetCells ?? new Dictionary<SheetCellKey, SheetCell>())
        {
            section.StoreOrPruneCell(kvp.Key, new SheetCell
            {
                RowType = kvp.Value.RowType,
                ColumnType = kvp.Value.ColumnType,
                CellLayout = kvp.Value.CellLayout,
                TextStyle = kvp.Value.TextStyle,
                ValueFormat = kvp.Value.ValueFormat,
                HideValue = kvp.Value.HideValue
            });
        }

        return section;
    }

    public SheetCell GetCell(RowType rowType, ColumnType columnType)
    {
        var key = new SheetCellKey(columnType, rowType);
        return SheetCells.TryGetValue(key, out var cell) ? cell : SheetCell.CreateDefault(columnType, rowType);
    }

    public void ApplyFormatPatch(RowType rowType, ColumnType columnType, ReportFormatPatch patch)
    {
        ArgumentNullException.ThrowIfNull(patch);

        var key = new SheetCellKey(columnType, rowType);
        var cell = SheetCells.TryGetValue(key, out var existing) ? existing : SheetCell.CreateDefault(columnType, rowType);

        cell.ApplyFormatPatch(patch);
        StoreOrPruneCell(key, cell);
    }
    private void StoreOrPruneCell(SheetCellKey key, SheetCell cell)
    {
        if (cell.IsDefault())
        {
            SheetCells.Remove(key);
            return;
        }

        SheetCells[key] = cell;
    }
    public IViewSection Clone()
    {
        var clone = new SheetSection
        {
            SectionId = SectionId.New(),
            SourceType = SourceType,
            EnableChanges = EnableChanges,
            PageBreakBehavior = PageBreakBehavior
        };
        foreach (var kvp in SheetCells)
        {
            clone.SheetCells[kvp.Key] = new SheetCell
            {
                RowType = kvp.Value.RowType,
                ColumnType = kvp.Value.ColumnType,
                CellLayout = kvp.Value.CellLayout,
                TextStyle = kvp.Value.TextStyle,
                ValueFormat = kvp.Value.ValueFormat,
                HideValue = kvp.Value.HideValue
            };
        }
        return clone;
    }
}
