using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Domain.Views.DataSections;

public class DataSection : IViewSection
{
    private List<RowDefinition> _rowDefinitions { get; set; } = new();
    private List<DataColumnDefinition> _columnDefinitions { get; set; } = new();
    private const double DefaultRowHeightMm = 8;
    private const double DefaultColumnRelativeWidth = 1;

    public SectionId Id { get; set; } = SectionId.New();
    /// <summary>
    /// Key: "R{rowIndex}C{columnIndex}" 0 based arrays
    /// </summary>
    public Dictionary<string, DataCell> DataCells { get; set; } = new();
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
                RowIndex = i,
                HeightMm = DefaultRowHeightMm
            });
        }

        for (var i = 0; i < columnsCount; i++)
        {
            section._columnDefinitions.Add(new DataColumnDefinition
            {
                ColumnIndex = i,
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
            RowIndex = RowsCount,
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
            ColumnIndex = ColumnsCount,
            RelativeWidth = relativeWidth
        };

        _columnDefinitions.Add(column);

        return column;
    }

    public void RemoveRow(int rowIndex)
    {
        ValidateRowIndex(rowIndex);

        if (RowsCount == 1)
            throw new InvalidOperationException("A data section cannot remove its last row.");

        var rowDefinition = _rowDefinitions.Single(x => x.RowIndex == rowIndex);
        _rowDefinitions.Remove(rowDefinition);

        foreach (var row in _rowDefinitions.Where(x => x.RowIndex > rowIndex))
            row.RowIndex--;

        var updatedCells = new Dictionary<string, DataCell>();
        foreach (var cell in DataCells.Values)
        {
            if (cell.RowIndex == rowIndex)
                continue;

            if (cell.RowIndex > rowIndex)
                cell.RowIndex--;

            updatedCells[BuildCellKey(cell.RowIndex, cell.ColumnIndex)] = cell;
        }

        DataCells = updatedCells;
        _legacyRowsCount = _rowDefinitions.Count;
    }

    public void RemoveColumn(int columnIndex)
    {
        Normalize();
        ValidateColumnIndex(columnIndex);

        if (ColumnsCount == 1)
            throw new InvalidOperationException("A data section cannot remove its last column.");

        var columnDefinition = _columnDefinitions.Single(x => x.ColumnIndex == columnIndex);
        _columnDefinitions.Remove(columnDefinition);

        foreach (var column in _columnDefinitions.Where(x => x.ColumnIndex > columnIndex))
            column.ColumnIndex--;

        var updatedCells = new Dictionary<string, DataCell>();
        foreach (var cell in DataCells.Values)
        {
            if (cell.ColumnIndex == columnIndex)
                continue;

            if (cell.ColumnIndex > columnIndex)
                cell.ColumnIndex--;

            updatedCells[BuildCellKey(cell.RowIndex, cell.ColumnIndex)] = cell;
        }

        DataCells = updatedCells;
        _legacyColumnsCount = _columnDefinitions.Count;
    }

    public DataCell GetCell(int rowIndex, int columnIndex)
    {
        Normalize();
        ValidateRowIndex(rowIndex);
        ValidateColumnIndex(columnIndex);

        return DataCells.TryGetValue(BuildCellKey(rowIndex, columnIndex), out var cell)
            ? cell
            : CreateDefaultCell(rowIndex, columnIndex);
    }

    public bool TryGetCell(int rowIndex, int columnIndex, out DataCell? cell)
    {
        Normalize();
        cell = null;

        if (rowIndex < 0 || rowIndex >= RowsCount || columnIndex < 0 || columnIndex >= ColumnsCount)
            return false;

        return DataCells.TryGetValue(BuildCellKey(rowIndex, columnIndex), out cell);
    }

    public void EditCellExpression(int rowIndex, int columnIndex, string? cellExpression)
    {
        Normalize();
        ValidateRowIndex(rowIndex);
        ValidateColumnIndex(columnIndex);

        var cell = GetStoredOrNewCell(rowIndex, columnIndex);
        cell.CellExpression = cellExpression ?? string.Empty;
        StoreOrPruneCell(cell);
    }

    public void ApplyFormatPatch(DataCellRange range, ReportFormatPatch patch)
    {
        ArgumentNullException.ThrowIfNull(patch);
        Normalize();
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
        if (DataCells.TryGetValue(key, out var cell))
            return cell;

        return CreateDefaultCell(rowIndex, columnIndex);
    }

    private void StoreOrPruneCell(DataCell cell)
    {
        var key = BuildCellKey(cell.RowIndex, cell.ColumnIndex);
        if (cell.IsDefault())
        {
            DataCells.Remove(key);
            return;
        }

        DataCells[key] = cell;
    }

    private void ValidateRowIndex(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= RowsCount)
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
    }

    private void ValidateColumnIndex(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= ColumnsCount)
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

    private static bool TryReadCellPosition(string key, out int rowIndex, out int columnIndex)
    {
        rowIndex = default;
        columnIndex = default;

        if (string.IsNullOrWhiteSpace(key) || key[0] != 'R')
            return false;

        var columnMarkerIndex = key.IndexOf('C', StringComparison.Ordinal);
        if (columnMarkerIndex <= 1 || columnMarkerIndex == key.Length - 1)
            return false;

        return int.TryParse(key[1..columnMarkerIndex], out rowIndex)
               && int.TryParse(key[(columnMarkerIndex + 1)..], out columnIndex);
    }

    public IViewSection Clone()
    {
        throw new NotImplementedException();
    }
}
