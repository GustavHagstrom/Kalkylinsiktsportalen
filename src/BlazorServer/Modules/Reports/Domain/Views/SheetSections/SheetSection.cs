using System.Collections.ObjectModel;

namespace BlazorServer.Modules.Reports.Domain.Views.SheetSections;

public class SheetSection : IViewSection
{
    public SectionId SectionId { get; private set; } = SectionId.New();
    public SheetSourceType SourceType { get; private set; } = SheetSourceType.TenderSheet;
    public bool ShowHeaders { get; set; } = true;
    public IReadOnlyList<ColumnDefinition> Columns { get; private set; } = DefaultColumns();
    public bool EnableChanges { get; set; } = false;
    public Dictionary<SheetCellKey, SheetCell> SheetCells { get; } = new();
    public PageBreakBehavior PageBreakBehavior { get; set; } = PageBreakBehavior.None;

    private SheetSection() { }
    public static SheetSection Create(SheetSourceType sourceType = SheetSourceType.TenderSheet, bool enableChanges = false, bool showHeaders = true, PageBreakBehavior pageBreakBehavior = PageBreakBehavior.None)
    {
        return new SheetSection
        {
            SectionId = SectionId.New(),
            SourceType = sourceType,
            ShowHeaders = showHeaders,
            EnableChanges = enableChanges,
            PageBreakBehavior = pageBreakBehavior
        };
    }
    internal static SheetSection Reconstitute(SectionId sectionId, IReadOnlyList<ColumnDefinition> columns, SheetSourceType sourceType = SheetSourceType.TenderSheet, bool enableChanges = false, bool showHeaders = true, IReadOnlyDictionary<SheetCellKey, SheetCell>? sheetCells = null, PageBreakBehavior pageBreakBehavior = PageBreakBehavior.None)
    {
        ArgumentNullException.ThrowIfNull(sectionId);

        var section = new SheetSection
        {
            SectionId = sectionId,
            SourceType = sourceType,
            ShowHeaders = showHeaders,
            EnableChanges = enableChanges,
            PageBreakBehavior = pageBreakBehavior
        };
        section.SetColumnDefinitions(columns);

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
    private void SetColumnDefinitions(IEnumerable<ColumnDefinition> columns)
    {
        var columnsToAdd = new Dictionary<ColumnType, ColumnDefinition>();
        ArgumentNullException.ThrowIfNull(columns);
        if (columns.Count() != Enum.GetValues<ColumnType>().Length)
        {
            throw new ReportsDomainException(
                "sheetsection.columns.invalid",
                $"The number of column definitions must match the number of column types. Expected {Enum.GetValues<ColumnType>().Length}, but got {columns.Count()}.");
        }
        foreach (var column in columns)
        {
            if (columnsToAdd.ContainsKey(column.ColumnType))
            {
                throw new ReportsDomainException(
                    "sheetsection.columns.duplicate",
                    $"Duplicate column definition for column type {column.ColumnType}.");
            }
            columnsToAdd[column.ColumnType] = column;
        }

        Columns = columnsToAdd.Values.ToList();
    }
    public IViewSection Clone()
    {
        var clone = new SheetSection
        {
            SectionId = SectionId.New(),
            SourceType = SourceType,
            ShowHeaders = ShowHeaders,
            EnableChanges = EnableChanges,
            PageBreakBehavior = PageBreakBehavior,
            Columns = Columns
                .Select(column => ColumnDefinition.Create(column.ColumnType, column.RelativeWidth, column.IsVisible))
                .ToList()
        };
        foreach (var kvp in SheetCells)
        {
            clone.SheetCells[kvp.Key] = new SheetCell
            {
                RowType = kvp.Value.RowType,
                ColumnType = kvp.Value.ColumnType,
                CellLayout = kvp.Value.CellLayout?.Clone(),
                TextStyle = kvp.Value.TextStyle?.Clone(),
                ValueFormat = kvp.Value.ValueFormat?.Clone(),
                HideValue = kvp.Value.HideValue
            };
        }
        return clone;
    }
    public static List<ColumnDefinition> DefaultColumns()
    {
        var columns = new List<ColumnDefinition>();
        foreach (var columnType in Enum.GetValues<ColumnType>())
        {
            columns.Add(ColumnDefinition.Create(columnType));
        }
        return columns;
    }
}
