using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Domain.Views.DataSections;

public class DataSection : IViewSection
{
    private List<RowDefinition> _rowDefinitions = [];
    private List<DataColumnDefinition> _columnDefinitions = [];
    private const double DefaultRowHeightMm = 8;
    private const double DefaultColumnRelativeWidth = 1;
    /// <summary>
    /// Key: "R{rowIndex}C{columnIndex}" 0 based arrays
    /// </summary>
    private Dictionary<string, DataCell> _dataCells = new();
    public SectionId Id { get; set; } = SectionId.New();
    public IReadOnlyList<RowDefinition> Rows => _rowDefinitions;
    public IReadOnlyList<DataColumnDefinition> Columns => _columnDefinitions;
    public static DataSection Create(int rowsCount, int columnsCount)
    {
        var section = new DataSection();

        if (rowsCount < 1)
            throw new ArgumentOutOfRangeException(nameof(rowsCount));

        if (columnsCount < 1)
            throw new ArgumentOutOfRangeException(nameof(columnsCount));


        for (var i = 0; i < rowsCount; i++)
        {
            section._rowDefinitions.Add(new RowDefinition
            {
                HeightMm = DefaultRowHeightMm
            });
        }

        for (var i = 0; i < columnsCount; i++)
        {
            section._columnDefinitions.Add(new DataColumnDefinition
            {
                RelativeWidth = DefaultColumnRelativeWidth
            });
        }

        return section;
    }

    public static DataSection CreateDefault() => Create(3, 3);

    public RowDefinition AddRow(double heightMm = DefaultRowHeightMm)
    {
        if (heightMm <= 0)
            throw new ArgumentOutOfRangeException(nameof(heightMm));

        var row = new RowDefinition
        {
            HeightMm = heightMm
        };

        _rowDefinitions.Add(row);

        return row;
    }

    public DataColumnDefinition AddColumn(double relativeWidth = DefaultColumnRelativeWidth)
    {
        if (relativeWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(relativeWidth));

        var column = new DataColumnDefinition
        {
            RelativeWidth = relativeWidth
        };

        _columnDefinitions.Add(column);

        return column;
    }

    public void RemoveRow(int rowIndex)
    {
        ValidateRowIndex(rowIndex);

        if (Rows.Count == 1)
            throw new InvalidOperationException("A data section cannot remove its last row.");

        _rowDefinitions.RemoveAt(rowIndex);

        var updatedCells = new Dictionary<string, DataCell>();
        foreach (var cell in _dataCells.Values)
        {
            if (cell.RowIndex == rowIndex)
                continue;

            if (cell.RowIndex > rowIndex)
                cell.RowIndex--;

            updatedCells[BuildCellKey(cell.RowIndex, cell.ColumnIndex)] = cell;
        }

        _dataCells = updatedCells;
    }

    public void RemoveColumn(int columnIndex)
    {
        ValidateColumnIndex(columnIndex);

        if (Columns.Count == 1)
            throw new InvalidOperationException("A data section cannot remove its last column.");

        _columnDefinitions.RemoveAt(columnIndex);

        var updatedCells = new Dictionary<string, DataCell>();
        foreach (var cell in _dataCells.Values)
        {
            if (cell.ColumnIndex == columnIndex)
                continue;

            if (cell.ColumnIndex > columnIndex)
                cell.ColumnIndex--;

            updatedCells[BuildCellKey(cell.RowIndex, cell.ColumnIndex)] = cell;
        }

        _dataCells = updatedCells;
    }

    public DataCell GetCell(int rowIndex, int columnIndex)
    {
        ValidateRowIndex(rowIndex);
        ValidateColumnIndex(columnIndex);

        return _dataCells.TryGetValue(BuildCellKey(rowIndex, columnIndex), out var cell) ? cell : CreateDefaultCell(rowIndex, columnIndex);
    }

    public bool TryGetCell(int rowIndex, int columnIndex, out DataCell? cell)
    {
        cell = null;

        if (rowIndex < 0 || rowIndex >= Rows.Count || columnIndex < 0 || columnIndex >= Columns.Count)
            return false;

        return _dataCells.TryGetValue(BuildCellKey(rowIndex, columnIndex), out cell);
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
        var key = BuildCellKey(rowIndex, columnIndex);
        if (_dataCells.TryGetValue(key, out var cell))
            return cell;

        return CreateDefaultCell(rowIndex, columnIndex);
    }

    private void StoreOrPruneCell(DataCell cell)
    {
        var key = BuildCellKey(cell.RowIndex, cell.ColumnIndex);
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

    private static DataCell CreateDefaultCell(int rowIndex, int columnIndex)
        => new()
        {
            RowIndex = rowIndex,
            ColumnIndex = columnIndex
        };

    private static string BuildCellKey(int rowIndex, int columnIndex) => $"R{rowIndex}C{columnIndex}";

    public IViewSection Clone()
    {
        throw new NotImplementedException();
    }
}
