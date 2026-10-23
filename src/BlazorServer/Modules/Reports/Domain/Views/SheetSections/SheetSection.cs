namespace BlazorServer.Modules.Reports.Domain.Views.SheetSections;

public class SheetSection : IViewSection
{
    private List<SheetColumnConfiguration> _columns = DefaultColumns();
    private readonly Dictionary<SheetCellKey, SheetCellConfiguration> _sheetCellConfigurations = new();

    public SectionId SectionId { get; private set; } = SectionId.New();
    public SheetKind SheetKind { get; private set; } = SheetKind.TenderSheet;
    public bool ShowHeaders { get; set; } = true;
    public IReadOnlyList<SheetColumnConfiguration> Columns => _columns.AsReadOnly();
    public bool EnableChanges { get; set; } = false;
    public IReadOnlyDictionary<SheetCellKey, SheetCellConfiguration> SheetCellConfigurations
        => new System.Collections.ObjectModel.ReadOnlyDictionary<SheetCellKey, SheetCellConfiguration>(_sheetCellConfigurations);
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
    internal static SheetSection Reconstitute(SectionId sectionId, IReadOnlyList<SheetColumnConfiguration> columns, SheetKind sheetKind = SheetKind.TenderSheet, bool enableChanges = false, bool showHeaders = true, IReadOnlyDictionary<SheetCellKey, SheetCellConfiguration>? sheetCellConfigurations = null, PageBreakBehavior pageBreakBehavior = PageBreakBehavior.None)
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
        section.ReplaceColumnConfigurations(columns);

        foreach (var kvp in sheetCellConfigurations ?? new Dictionary<SheetCellKey, SheetCellConfiguration>())
        {
            section.StoreOrPruneCell(kvp.Key, SheetCellConfiguration.Create(
                cellLayout: kvp.Value.CellLayout,
                textStyle: kvp.Value.TextStyle,
                valueFormat: kvp.Value.ValueFormat,
                hideValue: kvp.Value.HideValue));
        }

        return section;
    }
    public SheetCellConfiguration GetCell(RowType rowType, SheetColumnKind sheetColumnKind)
    {
        var key = new SheetCellKey(sheetColumnKind, rowType);
        return _sheetCellConfigurations.TryGetValue(key, out var cell) ? cell : SheetCellConfiguration.Create();
    }
    public void ApplyFormatPatch(RowType rowType, SheetColumnKind sheetColumnKind, ReportFormatPatch patch)
    {
        ArgumentNullException.ThrowIfNull(patch);

        var key = new SheetCellKey(sheetColumnKind, rowType);
        var cell = _sheetCellConfigurations.TryGetValue(key, out var existing) ? existing : SheetCellConfiguration.Create();

        cell = SheetCellConfiguration.CreateFromPatch(cell, patch);
        StoreOrPruneCell(key, cell);
    }
    public void SetColumnVisibility(SheetColumnKind sheetColumnKind, bool isVisible)
    {
        ReplaceColumnConfiguration(
            sheetColumnKind,
            column => column with { IsVisible = isVisible });
    }
    public void SetColumnRelativeWidth(SheetColumnKind sheetColumnKind, double relativeWidth)
    {
        ReplaceColumnConfiguration(
            sheetColumnKind,
            column => column with { RelativeWidth = relativeWidth });
    }
    public void MoveColumn(SheetColumnKind sheetColumnKind, int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= _columns.Count)
            throw new ArgumentOutOfRangeException(nameof(targetIndex));

        var columns = _columns.ToList();
        var currentIndex = columns.FindIndex(column => column.SheetColumnKind == sheetColumnKind);
        if (currentIndex < 0)
        {
            throw new ReportsDomainException(
                "sheetsection.columns.missing",
                $"Missing sheet column configuration for sheet column kind {sheetColumnKind}.");
        }

        var column = columns[currentIndex];
        columns.RemoveAt(currentIndex);
        columns.Insert(targetIndex, column);
        ReplaceColumnConfigurations(columns);
    }
    internal void ReplaceColumnConfigurations(IReadOnlyList<SheetColumnConfiguration> columns)
    {
        _columns = ValidateColumnConfigurations(columns);
    }
    private void StoreOrPruneCell(SheetCellKey key, SheetCellConfiguration cell)
    {
        if (cell.IsDefault())
        {
            _sheetCellConfigurations.Remove(key);
            return;
        }

        _sheetCellConfigurations[key] = cell;
    }
    private void ReplaceColumnConfiguration(
        SheetColumnKind sheetColumnKind,
        Func<SheetColumnConfiguration, SheetColumnConfiguration> replacement)
    {
        ArgumentNullException.ThrowIfNull(replacement);

        var currentIndex = _columns.FindIndex(column => column.SheetColumnKind == sheetColumnKind);
        if (currentIndex < 0)
        {
            throw new ReportsDomainException(
                "sheetsection.columns.missing",
                $"Missing sheet column configuration for sheet column kind {sheetColumnKind}.");
        }

        var columns = _columns.ToList();
        columns[currentIndex] = replacement(columns[currentIndex]);
        ReplaceColumnConfigurations(columns);
    }
    private static List<SheetColumnConfiguration> ValidateColumnConfigurations(IEnumerable<SheetColumnConfiguration> columns)
    {
        ArgumentNullException.ThrowIfNull(columns);
        var columnList = columns.ToList();
        var knownColumnKinds = Enum.GetValues<SheetColumnKind>();
        var columnsToAdd = new Dictionary<SheetColumnKind, SheetColumnConfiguration>();
        if (columnList.Count != knownColumnKinds.Length)
        {
            throw new ReportsDomainException(
                "sheetsection.columns.invalid",
                $"The number of sheet column configurations must match the number of sheet column kinds. Expected {knownColumnKinds.Length}, but got {columnList.Count}.");
        }
        foreach (var column in columnList)
        {
            if (!Enum.IsDefined(column.SheetColumnKind))
            {
                throw new ReportsDomainException(
                    "sheetsection.columns.unknown",
                    $"Unknown sheet column kind {column.SheetColumnKind}.");
            }

            if (columnsToAdd.ContainsKey(column.SheetColumnKind))
            {
                throw new ReportsDomainException(
                    "sheetsection.columns.duplicate",
                    $"Duplicate sheet column configuration for sheet column kind {column.SheetColumnKind}.");
            }
            columnsToAdd[column.SheetColumnKind] = column;
        }
        if (columnList.All(column => !column.IsVisible))
        {
            throw new ReportsDomainException(
                "sheetsection.columns.no_visible_columns",
                "A sheet section must have at least one visible column.");
        }

        return columnList;
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
            _columns = _columns
                .Select(column => column with { })
                .ToList()
        };
        foreach (var kvp in SheetCellConfigurations)
        {
            clone._sheetCellConfigurations[kvp.Key] = SheetCellConfiguration.Create(
                cellLayout: kvp.Value.CellLayout,
                textStyle: kvp.Value.TextStyle,
                valueFormat: kvp.Value.ValueFormat,
                hideValue: kvp.Value.HideValue);
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
