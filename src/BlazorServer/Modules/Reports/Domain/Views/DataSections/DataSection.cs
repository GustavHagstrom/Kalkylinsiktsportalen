using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Domain.Views.DataSections;

public class DataSection : IViewSection
{
    private const double DefaultRowHeightMm = 8;
    private const double DefaultColumnRelativeWidth = 1;

    private List<RowDefinition> _rowDefinitions = [];
    private List<ColumnDefinition> _columnDefinitions = [];
    private Dictionary<DataCellKey, DataCell> _dataCells = new();
    public SectionId SectionId { get; private set; } = SectionId.New();
    public PageBreakBehavior PageBreakBehavior { get; set; } = PageBreakBehavior.None;

    public IReadOnlyList<DataCell> Cells => _dataCells.Values.ToList();
    public IReadOnlyList<RowDefinition> Rows => _rowDefinitions;
    public IReadOnlyList<ColumnDefinition> Columns => _columnDefinitions;
    
    public static DataSection Create(int rowsCount, int columnsCount)
    {
        var section = new DataSection();

        if (rowsCount < 1)
            throw new ArgumentOutOfRangeException(nameof(rowsCount));

        if (columnsCount < 1)
            throw new ArgumentOutOfRangeException(nameof(columnsCount));


        for (var i = 0; i < rowsCount; i++)
        {
            section._rowDefinitions.Add(new RowDefinition(DefaultRowHeightMm));
        }

        for (var i = 0; i < columnsCount; i++)
        {
            section._columnDefinitions.Add(new ColumnDefinition(DefaultColumnRelativeWidth));
        }

        return section;
    }
    public static DataSection CreateDefault() => Create(3, 3);
    internal static DataSection Reconstitute(SectionId sectionId, IReadOnlyList<RowDefinition> rows, IReadOnlyList<ColumnDefinition> columns, IReadOnlyDictionary<DataCellKey, DataCell>? cells = null, PageBreakBehavior pageBreakBehavior = PageBreakBehavior.None)
    {
        ArgumentNullException.ThrowIfNull(sectionId);
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(columns);

        if (rows.Count < 1)
            throw new ArgumentOutOfRangeException(nameof(rows));

        if (columns.Count < 1)
            throw new ArgumentOutOfRangeException(nameof(columns));

        var section = new DataSection
        {
            SectionId = sectionId,
            PageBreakBehavior = pageBreakBehavior,
            _rowDefinitions = rows.Select(row => new RowDefinition(row.HeightMm)).ToList(),
            _columnDefinitions = columns.Select(column => new ColumnDefinition(column.RelativeWidth)).ToList()
        };

        foreach (var cell in cells?.Values ?? [])
        {
            section.ValidateRowIndex(cell.RowIndex);
            section.ValidateColumnIndex(cell.ColumnIndex);
            section.StoreOrPruneCell(new DataCell
            {
                RowIndex = cell.RowIndex,
                ColumnIndex = cell.ColumnIndex,
                CellExpression = cell.CellExpression,
                CellLayout = cell.CellLayout,
                TextStyle = cell.TextStyle,
                ValueFormat = cell.ValueFormat
            });
        }

        return section;
    }
    public RowDefinition AddRow(double heightMm = DefaultRowHeightMm)
    {
        if (heightMm <= 0)
            throw new ArgumentOutOfRangeException(nameof(heightMm));

        var row = new RowDefinition(heightMm);

        _rowDefinitions.Add(row);

        return row;
    }
    public ColumnDefinition AddColumn(double relativeWidth = DefaultColumnRelativeWidth)
    {
        if (relativeWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(relativeWidth));

        var column = new ColumnDefinition(relativeWidth);

        _columnDefinitions.Add(column);

        return column;
    }
    public void RemoveRow(int rowIndex)
    {
        ValidateRowIndex(rowIndex);

        if (Rows.Count == 1)
            throw new InvalidOperationException("A data section cannot remove its last row.");

        _rowDefinitions.RemoveAt(rowIndex);

        var updatedCells = new Dictionary<DataCellKey, DataCell>();
        foreach (var cell in _dataCells.Values)
        {
            if (cell.RowIndex == rowIndex)
                continue;

            if (cell.RowIndex > rowIndex)
                cell.RowIndex--;

            updatedCells[new DataCellKey(cell.RowIndex, cell.ColumnIndex)] = cell;
        }

        _dataCells = updatedCells;
    }
    public void RemoveColumn(int columnIndex)
    {
        ValidateColumnIndex(columnIndex);

        if (Columns.Count == 1)
            throw new InvalidOperationException("A data section cannot remove its last column.");

        _columnDefinitions.RemoveAt(columnIndex);

        var updatedCells = new Dictionary<DataCellKey, DataCell>();
        foreach (var cell in _dataCells.Values)
        {
            if (cell.ColumnIndex == columnIndex)
                continue;

            if (cell.ColumnIndex > columnIndex)
                cell.ColumnIndex--;

            updatedCells[new DataCellKey(cell.RowIndex, cell.ColumnIndex)] = cell;
        }

        _dataCells = updatedCells;
    }
    public DataCell GetCell(int rowIndex, int columnIndex)
    {
        ValidateRowIndex(rowIndex);
        ValidateColumnIndex(columnIndex);

        return _dataCells.TryGetValue(new DataCellKey(rowIndex, columnIndex), out var cell) ? cell : DataCell.CreateDefault(rowIndex, columnIndex);
    }
    public bool TryGetCell(int rowIndex, int columnIndex, out DataCell? cell)
    {
        cell = null;

        if (rowIndex < 0 || rowIndex >= Rows.Count || columnIndex < 0 || columnIndex >= Columns.Count)
            return false;

        return _dataCells.TryGetValue(new DataCellKey(rowIndex, columnIndex), out cell);
    }
    public void EditCellExpression(int rowIndex, int columnIndex, string? cellExpression)
    {
        ValidateRowIndex(rowIndex);
        ValidateColumnIndex(columnIndex);

        var cell = GetStoredOrNewCell(rowIndex, columnIndex);
        cell.CellExpression = cellExpression ?? string.Empty;
        StoreOrPruneCell(cell);
    }
    public void ApplyFormatPatch(DataCellRange range, ReportFormatPatch patch)
    {
        ArgumentNullException.ThrowIfNull(patch);
        ValidateRange(range);

        for (var rowIndex = range.StartRowIndex; rowIndex <= range.EndRowIndex; rowIndex++)
        {
            for (var columnIndex = range.StartColumnIndex; columnIndex <= range.EndColumnIndex; columnIndex++)
            {
                var cell = GetStoredOrNewCell(rowIndex, columnIndex);
                cell.ApplyFormatPatch(patch);
                StoreOrPruneCell(cell);
            }
        }
    }
    private DataCell GetStoredOrNewCell(int rowIndex, int columnIndex)
    {
        var key = new DataCellKey(rowIndex, columnIndex);
        if (_dataCells.TryGetValue(key, out var cell))
            return cell;

        return DataCell.CreateDefault(rowIndex, columnIndex);
    }
    private void StoreOrPruneCell(DataCell cell)
    {
        var key = new DataCellKey(cell.RowIndex, cell.ColumnIndex);
        if (cell.IsDefault())
        {
            _dataCells.Remove(key);
            return;
        }

        _dataCells[key] = cell;
    }
    private void ValidateRowIndex(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= Rows.Count)
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
    }
    private void ValidateColumnIndex(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= Columns.Count)
            throw new ArgumentOutOfRangeException(nameof(columnIndex));
    }
    private void ValidateRange(DataCellRange range)
    {
        if (range.StartRowIndex > range.EndRowIndex)
            throw new ArgumentException("Range start row must be before end row.", nameof(range));

        if (range.StartColumnIndex > range.EndColumnIndex)
            throw new ArgumentException("Range start column must be before end column.", nameof(range));

        ValidateRowIndex(range.StartRowIndex);
        ValidateRowIndex(range.EndRowIndex);
        ValidateColumnIndex(range.StartColumnIndex);
        ValidateColumnIndex(range.EndColumnIndex);
    }
    public IViewSection Clone()
    {
        return new DataSection
        {
            SectionId = SectionId.New(),
            PageBreakBehavior = this.PageBreakBehavior,
            _rowDefinitions = this._rowDefinitions.Select(r => new RowDefinition(r.HeightMm)).ToList(),
            _columnDefinitions = this._columnDefinitions.Select(c => new ColumnDefinition(c.RelativeWidth)).ToList(),
            _dataCells = this._dataCells.ToDictionary(kvp => kvp.Key, kvp => new DataCell
            {
                RowIndex = kvp.Value.RowIndex,
                ColumnIndex = kvp.Value.ColumnIndex,
                CellLayout = kvp.Value.CellLayout?.Clone(),
                TextStyle = kvp.Value.TextStyle?.Clone(),
                ValueFormat = kvp.Value.ValueFormat?.Clone(),
                CellExpression = kvp.Value.CellExpression
            })
        };
    }
}
