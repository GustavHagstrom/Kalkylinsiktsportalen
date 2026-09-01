namespace BlazorServer.Modules.Reports.Domain.Views.SheetSections;

public class SheetSection : IViewSection
{
    public SectionId SectionId { get; private set; } = SectionId.New();
    public SheetKind SheetKind { get; private set; } = SheetKind.TenderSheet;
    public bool ShowHeaders { get; set; } = true;
    public IReadOnlyList<SheetColumnConfiguration> Columns { get; private set; } = DefaultColumns();
    public bool EnableChanges { get; set; } = false;
    public Dictionary<SheetCellKey, SheetCell> SheetCells { get; } = new();
    public PageBreakBehavior PageBreakBehavior { get; set; } = PageBreakBehavior.None;

    private SheetSection() { }
    public static SheetSection Create(SheetKind sheetKind = SheetKind.TenderSheet, bool enableChanges = false, bool showHeaders = true, PageBreakBehavior pageBreakBehavior = PageBreakBehavior.None)
    {
        return new SheetSection
        {
            SectionId = SectionId.New(),
            SheetKind = sheetKind,
            ShowHeaders = showHeaders,
            EnableChanges = enableChanges,
            PageBreakBehavior = pageBreakBehavior
        };
    }
    internal static SheetSection Reconstitute(SectionId sectionId, IReadOnlyList<SheetColumnConfiguration> columns, SheetKind sheetKind = SheetKind.TenderSheet, bool enableChanges = false, bool showHeaders = true, IReadOnlyDictionary<SheetCellKey, SheetCell>? sheetCells = null, PageBreakBehavior pageBreakBehavior = PageBreakBehavior.None)
    {
        ArgumentNullException.ThrowIfNull(sectionId);

        var section = new SheetSection
        {
            SectionId = sectionId,
            SheetKind = sheetKind,
            ShowHeaders = showHeaders,
            EnableChanges = enableChanges,
            PageBreakBehavior = pageBreakBehavior
        };
        section.SetColumnConfigurations(columns);

        foreach (var kvp in sheetCells ?? new Dictionary<SheetCellKey, SheetCell>())
        {
            section.StoreOrPruneCell(kvp.Key, new SheetCell
            {
                RowType = kvp.Value.RowType,
                SheetColumnKind = kvp.Value.SheetColumnKind,
                CellLayout = kvp.Value.CellLayout,
                TextStyle = kvp.Value.TextStyle,
                ValueFormat = kvp.Value.ValueFormat,
                HideValue = kvp.Value.HideValue
            });
        }

        return section;
    }
    public SheetCell GetCell(RowType rowType, SheetColumnKind sheetColumnKind)
    {
        var key = new SheetCellKey(sheetColumnKind, rowType);
        return SheetCells.TryGetValue(key, out var cell) ? cell : SheetCell.CreateDefault(sheetColumnKind, rowType);
    }
    public void ApplyFormatPatch(RowType rowType, SheetColumnKind sheetColumnKind, ReportFormatPatch patch)
    {
        ArgumentNullException.ThrowIfNull(patch);

        var key = new SheetCellKey(sheetColumnKind, rowType);
        var cell = SheetCells.TryGetValue(key, out var existing) ? existing : SheetCell.CreateDefault(sheetColumnKind, rowType);

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
    private void SetColumnConfigurations(IEnumerable<SheetColumnConfiguration> columns)
    {
        var columnsToAdd = new Dictionary<SheetColumnKind, SheetColumnConfiguration>();
        ArgumentNullException.ThrowIfNull(columns);
        if (columns.Count() != Enum.GetValues<SheetColumnKind>().Length)
        {
            throw new ReportsDomainException(
                "sheetsection.columns.invalid",
                $"The number of sheet column configurations must match the number of sheet column kinds. Expected {Enum.GetValues<SheetColumnKind>().Length}, but got {columns.Count()}.");
        }
        foreach (var column in columns)
        {
            if (columnsToAdd.ContainsKey(column.SheetColumnKind))
            {
                throw new ReportsDomainException(
                    "sheetsection.columns.duplicate",
                    $"Duplicate sheet column configuration for sheet column kind {column.SheetColumnKind}.");
            }
            columnsToAdd[column.SheetColumnKind] = column;
        }

        Columns = columnsToAdd.Values.ToList();
    }
    public IViewSection Clone()
    {
        var clone = new SheetSection
        {
            SectionId = SectionId.New(),
            SheetKind = SheetKind,
            ShowHeaders = ShowHeaders,
            EnableChanges = EnableChanges,
            PageBreakBehavior = PageBreakBehavior,
            Columns = Columns
                .Select(column => column with { })
                .ToList()
        };
        foreach (var kvp in SheetCells)
        {
            clone.SheetCells[kvp.Key] = new SheetCell
            {
                RowType = kvp.Value.RowType,
                SheetColumnKind = kvp.Value.SheetColumnKind,
                CellLayout = kvp.Value.CellLayout,
                TextStyle = kvp.Value.TextStyle,
                ValueFormat = kvp.Value.ValueFormat,
                HideValue = kvp.Value.HideValue
            };
        }
        return clone;
    }
    public static List<SheetColumnConfiguration> DefaultColumns()
    {
        var columns = new List<SheetColumnConfiguration>();
        foreach (var sheetColumnKind in Enum.GetValues<SheetColumnKind>())
        {
            columns.Add(SheetColumnConfiguration.Create(sheetColumnKind));
        }
        return columns;
    }
}
