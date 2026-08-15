using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Domain.Views.DataSection;

public class DataSection
{
    private const double DefaultRowHeightMm = 8;
    private const double DefaultColumnRelativeWidth = 1;
    private int _legacyRowsCount;
    private int _legacyColumnsCount;

    public string SectionId { get; set; } = SectionIds.New();
    public int SectionIndex { get; set; } = 0;
    public int RowsCount
    {
        get => RowDefinitions?.Count ?? _legacyRowsCount;
        set => _legacyRowsCount = value;
    }
    public int ColumnsCount
    {
        get => ColumnDefinitions?.Count ?? _legacyColumnsCount;
        set => _legacyColumnsCount = value;
    }
    /// <summary>
    /// Key: "R{rowIndex}C{columnIndex}" 0 based arrays
    /// </summary>
    public Dictionary<string, DataCell> DataCells { get; set; } = new();
    public List<RowDefinition> RowDefinitions { get; set; } = new();
    public List<DataColumnDefinition> ColumnDefinitions { get; set; } = new();

    public IReadOnlyList<RowDefinition> Rows => RowDefinitions;
    public IReadOnlyList<DataColumnDefinition> Columns => ColumnDefinitions;

    public static DataSection Create(int rowsCount, int columnsCount, int sectionIndex = 0)
    {
        var section = new DataSection
        {
            SectionIndex = sectionIndex
        };

        section.Initialize(rowsCount, columnsCount);
        return section;
    }

    public static DataSection CreateDefault(int sectionIndex)
        => Create(3, 3, sectionIndex);

    public void Initialize(int rowsCount, int columnsCount)
    {
        if (rowsCount < 1)
            throw new ArgumentOutOfRangeException(nameof(rowsCount));

        if (columnsCount < 1)
            throw new ArgumentOutOfRangeException(nameof(columnsCount));

        RowDefinitions.Clear();
        ColumnDefinitions.Clear();
        DataCells.Clear();
        _legacyRowsCount = rowsCount;
        _legacyColumnsCount = columnsCount;

        for (var i = 0; i < rowsCount; i++)
        {
            RowDefinitions.Add(new RowDefinition
            {
                RowIndex = i,
                HeightMm = DefaultRowHeightMm
            });
        }

        for (var i = 0; i < columnsCount; i++)
        {
            ColumnDefinitions.Add(new DataColumnDefinition
            {
                ColumnIndex = i,
                RelativeWidth = DefaultColumnRelativeWidth
            });
        }
    }

    public RowDefinition AddRow(double heightMm = DefaultRowHeightMm)
    {
        Normalize();

        if (heightMm <= 0)
            throw new ArgumentOutOfRangeException(nameof(heightMm));

        var row = new RowDefinition
        {
            RowIndex = RowsCount,
            HeightMm = heightMm
        };

        RowDefinitions.Add(row);

        return row;
    }

    public DataColumnDefinition AddColumn(double relativeWidth = DefaultColumnRelativeWidth)
    {
        Normalize();

        if (relativeWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(relativeWidth));

        var column = new DataColumnDefinition
        {
            ColumnIndex = ColumnsCount,
            RelativeWidth = relativeWidth
        };

        ColumnDefinitions.Add(column);

        return column;
    }

    public void RemoveRow(int rowIndex)
    {
        Normalize();
        ValidateRowIndex(rowIndex);

        if (RowsCount == 1)
            throw new InvalidOperationException("A data section cannot remove its last row.");

        var rowDefinition = RowDefinitions.Single(x => x.RowIndex == rowIndex);
        RowDefinitions.Remove(rowDefinition);

        foreach (var row in RowDefinitions.Where(x => x.RowIndex > rowIndex))
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
        _legacyRowsCount = RowDefinitions.Count;
    }

    public void RemoveColumn(int columnIndex)
    {
        Normalize();
        ValidateColumnIndex(columnIndex);

        if (ColumnsCount == 1)
            throw new InvalidOperationException("A data section cannot remove its last column.");

        var columnDefinition = ColumnDefinitions.Single(x => x.ColumnIndex == columnIndex);
        ColumnDefinitions.Remove(columnDefinition);

        foreach (var column in ColumnDefinitions.Where(x => x.ColumnIndex > columnIndex))
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
        _legacyColumnsCount = ColumnDefinitions.Count;
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

    public void Normalize()
    {
        if (string.IsNullOrWhiteSpace(SectionId))
            SectionId = SectionIds.New();

        RowDefinitions ??= new List<RowDefinition>();
        ColumnDefinitions ??= new List<DataColumnDefinition>();
        DataCells ??= new Dictionary<string, DataCell>();

        _legacyRowsCount = Math.Max(_legacyRowsCount, RowDefinitions.Count);
        _legacyColumnsCount = Math.Max(_legacyColumnsCount, ColumnDefinitions.Count);

        if (_legacyRowsCount < 1)
            _legacyRowsCount = 1;

        if (_legacyColumnsCount < 1)
            _legacyColumnsCount = 1;

        NormalizeRows();
        NormalizeColumns();
        NormalizeCells();
    }

    private void NormalizeRows()
    {
        var existingRows = RowDefinitions
            .Where(x => x.RowIndex >= 0)
            .GroupBy(x => x.RowIndex)
            .ToDictionary(x => x.Key, x => x.First());

        RowDefinitions = Enumerable.Range(0, _legacyRowsCount)
            .Select(index =>
            {
                if (existingRows.TryGetValue(index, out var row))
                {
                    row.RowIndex = index;
                    if (row.HeightMm <= 0)
                        row.HeightMm = DefaultRowHeightMm;

                    return row;
                }

                return new RowDefinition
                {
                    RowIndex = index,
                    HeightMm = DefaultRowHeightMm
                };
            })
            .ToList();

        _legacyRowsCount = RowDefinitions.Count;
    }

    private void NormalizeColumns()
    {
        var existingColumns = ColumnDefinitions
            .Where(x => x.ColumnIndex >= 0)
            .GroupBy(x => x.ColumnIndex)
            .ToDictionary(x => x.Key, x => x.First());

        ColumnDefinitions = Enumerable.Range(0, _legacyColumnsCount)
            .Select(index =>
            {
                if (existingColumns.TryGetValue(index, out var column))
                {
                    column.ColumnIndex = index;
                    if (column.RelativeWidth <= 0)
                        column.RelativeWidth = DefaultColumnRelativeWidth;

                    return column;
                }

                return new DataColumnDefinition
                {
                    ColumnIndex = index,
                    RelativeWidth = DefaultColumnRelativeWidth
                };
            })
            .ToList();

        _legacyColumnsCount = ColumnDefinitions.Count;
    }

    private void NormalizeCells()
    {
        var normalizedCells = new Dictionary<string, DataCell>();

        foreach (var keyedCell in DataCells)
        {
            var cell = keyedCell.Value;
            if (!TryReadCellPosition(keyedCell.Key, out var rowIndex, out var columnIndex))
            {
                rowIndex = cell.RowIndex;
                columnIndex = cell.ColumnIndex;
            }

            cell.RowIndex = rowIndex;
            cell.ColumnIndex = columnIndex;
            cell.ApplyLegacyFormatting();

            if (cell.RowIndex < 0 || cell.RowIndex >= RowsCount || cell.ColumnIndex < 0 || cell.ColumnIndex >= ColumnsCount)
                continue;

            if (cell.IsDefault())
                continue;

            normalizedCells[BuildCellKey(cell.RowIndex, cell.ColumnIndex)] = cell;
        }

        DataCells = normalizedCells;
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
}
